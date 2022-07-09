using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using InforceTask.Contracts.Requests;
using InforceTask.DataAccess.Repositories;
using InforceTask.Options;
using Microsoft.IdentityModel.Tokens;

namespace InforceTask.Domain.Services;

internal sealed class UserService : IUserService
{
	/// <summary>
	///     Source <see href="https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#pbkdf2" />
	/// </summary>
	private const int PasswordHashingIterationCount = 120000;

	/// <summary>
	///     Source <see href="https://nvlpubs.nist.gov/nistpubs/Legacy/SP/nistspecialpublication800-132.pdf" /> page 6
	/// </summary>
	private const int SaltSizeInBytes = 16;

	private static readonly HashAlgorithmName HashAlgorithmName = HashAlgorithmName.SHA512;
	private readonly IUserRepository _userRepository;
	private readonly IRefreshTokenRepository refreshTokenRepository;
	private readonly JwtOptions _jwtOptions;
	private readonly TokenValidationParameters _tokenValidationParameters;

	public UserService(IUserRepository userRepository,
					   IRefreshTokenRepository refreshTokenRepository, JwtOptions jwtOptions,
					   TokenValidationParameters tokenValidationParameters)
	{
		this._userRepository = userRepository;
		this.refreshTokenRepository = refreshTokenRepository;
		this._jwtOptions = jwtOptions;
		this._tokenValidationParameters = tokenValidationParameters;
	}

	public async Task<AuthResult> LoginUserAsync(LoginRequest request, Guid userId, CancellationToken cancellationToken = default)
	{
		var user = await this._userRepository.GetByIdAsync(userId, cancellationToken);

		if (user is not null)
		{
			if (CreatePasswordHash(request.Password, user.PasswordSalt) == user.PasswordHash)
			{
				return await this.CreateTokensAsync(user, cancellationToken);
			}
		}

		return AuthResult.FromFailure("User with this Username/Password combination wasn't found");
	}

	public async Task<AuthResult> RegisterUserAsync(LoginRequest request, CancellationToken cancellationToken = default)
	{
		var user = await this._userRepository.GetByUsernameAsync(request.Username, cancellationToken);
		if (user is not null)
		{
			return await LoginUserAsync(request, user.Id, cancellationToken);
		}

		var saltBuffer = new byte[SaltSizeInBytes];
		RandomNumberGenerator.Fill(saltBuffer);
		user = new User
		{
			Username = request.Username,
			PasswordHash = CreatePasswordHash(request.Password, saltBuffer),
			PasswordSalt = Convert.ToBase64String(saltBuffer),
			IsAdmin = request.ShouldBeAdmin
		};
		await this._userRepository.AddUserAsync(user, cancellationToken);
		await this._userRepository.SaveChangesAsync(cancellationToken);
		return await this.CreateTokensAsync(user, cancellationToken);
	}

	public async Task<AuthResult> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default)
	{
		var principal = this.GetClaimsPrincipalFromToken(accessToken);
		if (principal is null)
		{
			return AuthResult.FromFailure("Access token is invalid");
		}

		var now = DateTime.UtcNow;
		var expiryDateUnixTimestamp = long.Parse(principal.Claims.First(x => x.Type == ClaimTypes.JwtExpiration).Value);
		var expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(expiryDateUnixTimestamp).UtcDateTime;
		if (expiryDateTime > now)
		{
			return AuthResult.FromFailure("Access token haven't expired yet");
		}

		var refreshTokenRepository = this.refreshTokenRepository;
		var storedRefreshToken = await refreshTokenRepository.GetRefreshTokenByValueAsync(refreshToken, cancellationToken);
		if (storedRefreshToken is null)
		{
			return AuthResult.FromFailure("This refresh token doesn't exist");
		}

		if (now > storedRefreshToken.ExpiryDate)
		{
			return AuthResult.FromFailure("This refresh token has expired");
		}

		var jti = principal.Claims.First(x => x.Type == ClaimTypes.JwtId).Value;
		if (storedRefreshToken.JwtId != jti)
		{
			return AuthResult.FromFailure("Refresh token doesn't match access token");
		}

		var userId = Guid.Parse(principal.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
		var user = await this._userRepository.GetByIdAsync(userId, cancellationToken)!;
		return await this.CreateTokensAsync(user, cancellationToken);
	}

	private ClaimsPrincipal? GetClaimsPrincipalFromToken(string token)
	{
		var tokenHandler = new JwtSecurityTokenHandler();

		try
		{
			var tokenValidationParameters = this._tokenValidationParameters.Clone()!;
			tokenValidationParameters.ValidateLifetime = false;
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
			if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
			{
				return null;
			}

			return principal;
		}
		catch
		{
			return null;
		}
	}

	private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken) => validatedToken is JwtSecurityToken jwtSecurityToken &&
																						 jwtSecurityToken.Header.Alg.Equals(
																							 SecurityAlgorithms.HmacSha256,
																							 StringComparison.InvariantCultureIgnoreCase);


	private async Task<AuthResult> CreateTokensAsync(User user, CancellationToken cancellationToken = default)
	{
		var now = DateTime.UtcNow;
		var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();


		var claims = new List<Claim>
		{
			new(ClaimTypes.JwtSubject, user.Id.ToString()),
			new(ClaimTypes.JwtId, Guid.NewGuid().ToString()),
			new(ClaimTypes.Username, user.Username)
		};
		if (user.IsAdmin)
		{
			claims.Add(new(ClaimTypes.Admin, true.ToString()));
		}

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new(claims),
			Expires = now.Add(this._jwtOptions.AccessTokenLifetime),
			SigningCredentials = new(this._jwtOptions.GetSecurityKey(), SecurityAlgorithms.HmacSha256Signature)
		};
		var token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);

		var refreshToken = new RefreshToken
		{
			Token = Guid.NewGuid().ToString(),
			JwtId = token.Id,
			UserId = user.Id,
			CreationDate = now,
			ExpiryDate = now.Add(this._jwtOptions.RefreshTokenLifetime)
		};
		await this.refreshTokenRepository.AddTokenAsync(refreshToken, cancellationToken);
		await this.refreshTokenRepository.SaveChangesAsync(cancellationToken);
		return new()
		{
			Data = new()
			{
				AccessToken = jwtSecurityTokenHandler.WriteToken(token),
				RefreshToken = refreshToken.Token
			}
		};
	}

	private static string CreatePasswordHash(string password, ReadOnlySpan<byte> salt) =>
		Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(password, salt, PasswordHashingIterationCount, HashAlgorithmName, 16));

	private static string CreatePasswordHash(string password, string salt) => CreatePasswordHash(password, Convert.FromBase64String(salt));
}