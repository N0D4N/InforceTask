using InforceTask.Contracts.Requests;

namespace InforceTask.Domain.Services;

public interface IUserService
{
	Task<AuthResult> LoginUserAsync(LoginRequest request, Guid userId, CancellationToken cancellationToken = default);
	Task<AuthResult> RegisterUserAsync(LoginRequest request, CancellationToken cancellationToken = default);
	Task<AuthResult> RefreshTokenAsync(string accessToken, string refreshToken,CancellationToken cancellationToken = default );
}