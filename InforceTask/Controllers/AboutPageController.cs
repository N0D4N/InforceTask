using InforceTask.Contracts.Responses;
using InforceTask.Services;
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
	public IActionResult Edit([FromBody] string contents)
	{
		if (!this.User.HasClaim(x => x.Type == Constants.ClaimTypes.Admin))
		{
			return this.Unauthorized();
		}

		var result = this._aboutRedactorService.ChangeAboutPageContentsTo(contents);
		return this.Ok(new AboutPageContentsResponse()
		{
			Content = result.Content,
			EditNumber = result.EditNumber,
			LastEditDateTime = result.LastEditDateTime
		});
	}

	[HttpGet(Constants.Routes.About.Current)]
	public IActionResult Current()
	{
		var result = this._aboutRedactorService.GetCurrentAboutPageContents();
		return this.Ok(new AboutPageContentsResponse()
		{
			Content = result.Content,
			EditNumber = result.EditNumber,
			LastEditDateTime = result.LastEditDateTime
		});
	}
}