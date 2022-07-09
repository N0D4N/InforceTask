using InforceTask.Contracts.Requests;
using InforceTask.Contracts.Responses;
using InforceTask.Domain;
using InforceTask.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InforceTask.Controllers;

public sealed class AboutPageController : InforceBaseApiController
{
	private readonly IAboutRedactorService _aboutRedactorService;

	public AboutPageController(IAboutRedactorService aboutRedactorService)
	{
		this._aboutRedactorService = aboutRedactorService;
	}

	[Authorize]
	[HttpPost(Constants.Routes.About.Edit)]
	public async Task<IActionResult> EditAsync(EditAboutPageRequest request)
	{
		if (!this.User.HasClaim(x => x.Type == ClaimTypes.Admin))
		{
			return this.Unauthorized();
		}

		var result = await this._aboutRedactorService.ChangeAboutPageContentsAsync(request);
		return this.Ok(new AboutPageContentsResponse()
		{
			Content = result.Content,
			EditNumber = result.EditNumber,
			LastEditDateTime = result.LastEditDateTime
		});
	}

	[HttpGet(Constants.Routes.About.Current)]
	public async Task<IActionResult> CurrentAsync()
	{
		var result = await this._aboutRedactorService.GetCurrentAboutPageContentsAsync();
		return this.Ok(new AboutPageContentsResponse()
		{
			Content = result.Content,
			EditNumber = result.EditNumber,
			LastEditDateTime = result.LastEditDateTime
		});
	}
}