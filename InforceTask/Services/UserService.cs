using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using InforceTask.Contracts.Requests;
using InforceTask.Data;
using InforceTask.Models;
using InforceTask.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InforceTask.Services;

public sealed class UserService : IUserService
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
	private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
	private readonly JwtOptions _jwtOptions;
	private readonly TokenValidationParameters _tokenValidationParameters;

	public UserService(IDbContextFactory<AppDbContext> dbContextFactory, JwtOptions jwtOptions, TokenValidationParameters tokenValidationParameters)
	{
		this._dbContextFactory = dbContextFactory;
		this._jwtOptions = jwtOptions;
		this._tokenValidationParameters = tokenValidationParameters;
	}

	public AuthResult LoginUser(LoginRequest request, Guid userId)
	{
		using var appDbContext = this._dbContextFactory.CreateDbContext();
		var user = appDbContext.Users.FirstOrDefault(u => u.Id == userId);

		if (user is not null)
		{
			if (CreatePasswordHash(request.Password, user.PasswordSalt) == user.PasswordHash)
			{
				return this.CreateTokens(user);
			}
		}

		return AuthResult.FromFailure("User with this Username/Password combination wasn't found");
	}

	public AuthResult RegisterUser(LoginRequest request)
	{
		using var appDbContext = this._dbContextFactory.CreateDbContext();

		if (appDbContext.Users.Any(x => x.Username.Equals(request.Username)))
		{
			return AuthResult.FromFailure("This username is taken already");
		}

		Span<byte> saltBuffer = stackalloc byte[SaltSizeInBytes];
		RandomNumberGenerator.Fill(saltBuffer);
		var user = new User
		{
			Username = request.Username,
			PasswordHash = CreatePasswordHash(request.Password, saltBuffer),
			PasswordSalt = Convert.ToBase64String(saltBuffer),
			IsAdmin = request.ShouldBeAdmin
		};
		appDbContext.Users.Add(user);
		appDbContext.SaveChanges();
		return this.CreateTokens(user);
	}

	public AuthResult RefreshToken(string accessToken, string refreshToken)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var principal = this.GetClaimsPrincipalFromToken(accessToken);
		if (principal is null)
		{
			return AuthResult.FromFailure("Access token is invalid");
		}

		var now = DateTime.UtcNow;
		var expiryDateUnixTimestamp = long.Parse(principal.Claims.First(x => x.Type == Constants.ClaimTypes.JwtExpiration).Value);
		var expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(expiryDateUnixTimestamp).UtcDateTime;
		if (expiryDateTime > now)
		{
			return AuthResult.FromFailure("Access token haven't expired yet");
		}

		using var dbContext = this._dbContextFactory.CreateDbContext();
		var storedRefreshToken = dbContext.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);
		if (storedRefreshToken is null)
		{
			return AuthResult.FromFailure("This refresh token doesn't exist");
		}

		if (now > storedRefreshToken.ExpiryDate)
		{
			return AuthResult.FromFailure("This refresh token has expired");
		}

		var jti = principal.Claims.First(x => x.Type == Constants.ClaimTypes.JwtId).Value;
		if (storedRefreshToken.JwtId != jti)
		{
			return AuthResult.FromFailure("Refresh token doesn't match access token");
		}

		var userId = Guid.Parse(principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
		var user = dbContext.Users.First(x => x.Id == userId);
		return this.CreateTokens(user);
	}

	private ClaimsPrincipal? GetClaimsPrincipalFromToken(string token)
	{
		var tokenHandler = new JwtSecurityTokenHandler();

		try
		{
			var tokenValidationParameters = this._tokenValidationParameters.Clone();
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


	private AuthResult CreateTokens(User user)
	{
		var now = DateTime.UtcNow;
		var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();


		var claims = new List<Claim>
		{
			new(Constants.ClaimTypes.JwtSubject, user.Id.ToString()),
			new(Constants.ClaimTypes.JwtId, Guid.NewGuid().ToString()),
			new(Constants.ClaimTypes.Username, user.Username)
		};
		if (user.IsAdmin)
		{
			claims.Add(new(Constants.ClaimTypes.Admin, true.ToString()));
		}

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new(claims),
			Expires = now.Add(this._jwtOptions.AccessTokenLifetime),
			SigningCredentials = new(this._jwtOptions.Key, SecurityAlgorithms.HmacSha256Signature)
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
		using var dbContext = this._dbContextFactory.CreateDbContext();
		dbContext.RefreshTokens.Add(refreshToken);
		dbContext.SaveChanges();
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