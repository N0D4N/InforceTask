using InforceTask.Contracts.Requests;
using InforceTask.Contracts.Responses;
using InforceTask.Extensions;
using InforceTask.Models;
using InforceTask.Services;
using Microsoft.AspNetCore.Mvc;

namespace InforceTask.Controllers;

public sealed class UserController : InforceBaseApiController
{
	private readonly IUserService _userService;

	public UserController(IUserService userService)
	{
		this._userService = userService;
	}

	[HttpPost(Constants.Routes.Users.Login)]
	[ProducesResponseType(typeof(AuthResponse), 200)]
	[ProducesErrorResponseType(typeof(FailedResponse))]
	public IActionResult Login(LoginRequest request)
	{
		var userId = this.HttpContext.GetUserId();
		AuthResult authResult;
		if (userId is null)
		{
			authResult = this._userService.RegisterUser(request);
		}
		else
		{
			authResult = this._userService.LoginUser(request, userId.Value);
		}

		return this.MapDomainResult<TokensPair, AuthResponse>(authResult);
	}

	[HttpPost(Constants.Routes.Users.Refresh)]
	[ProducesResponseType(typeof(AuthResponse), 200)]
	[ProducesErrorResponseType(typeof(FailedResponse))]
	public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
	{
		var authResult = this._userService.RefreshToken(request.AccessToken, request.RefreshToken);
		return this.MapDomainResult<TokensPair, AuthResponse>(authResult);
	}
}