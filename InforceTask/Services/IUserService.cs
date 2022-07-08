using InforceTask.Contracts.Requests;
using InforceTask.Models;

namespace InforceTask.Services;

public interface IUserService
{
	AuthResult LoginUser(LoginRequest request, Guid userId);
	AuthResult RegisterUser(LoginRequest request);
	AuthResult RefreshToken(string accessToken, string refreshToken);
}