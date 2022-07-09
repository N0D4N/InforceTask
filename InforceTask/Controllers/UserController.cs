using InforceTask.Contracts.Requests;
using InforceTask.Contracts.Responses;
using InforceTask.Domain;
using InforceTask.Domain.Services;
using InforceTask.Extensions;
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
	public async Task<IActionResult> LoginAsync(LoginRequest request)
	{
		var userId = this.HttpContext.GetUserId();
		AuthResult authResult;
		if (userId is null)
		{
			authResult = await this._userService.RegisterUserAsync(request);
		}
		else
		{
			authResult = await this._userService.LoginUserAsync(request, userId.Value);
		}

		return this.MapAuthResult(authResult);
	}

	[HttpPost(Constants.Routes.Users.Refresh)]
	[ProducesResponseType(typeof(AuthResponse), 200)]
	[ProducesErrorResponseType(typeof(FailedResponse))]
	public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
	{
		var authResult = await this._userService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);
		return this.MapAuthResult(authResult);
	}

	private IActionResult MapAuthResult(AuthResult authResult)
	{
		if (authResult.Success)
		{
			return this.Ok(new AuthResponse()
			{
				Data = authResult.Data.ToTokensPairResponse()
			});
		}

		return this.BadRequest(new FailedResponse
		{
			Errors = authResult.Errors
		});

	}
}