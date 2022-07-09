using InforceTask.Contracts.Responses;
using InforceTask.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InforceTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class InforceBaseApiController : ControllerBase
{
	protected IActionResult MapDomainResult(DomainResult domainResult)
	{
		if (domainResult.Success)
		{
			return this.Ok();
		}

		return this.BadRequest(new FailedResponse
		{
			Errors = domainResult.Errors
		});
	}

	protected IActionResult MapDomainResult<TResponseData, TSuccessfulResponse>(DomainResult<TResponseData> domainResult)
		where TSuccessfulResponse : SuccessfulResponse<TResponseData>, new()
	{
		if (domainResult.Success)
		{
			return this.Ok(new TSuccessfulResponse
			{
				Data = domainResult.Data
			});
		}

		return this.BadRequest(new FailedResponse
		{
			Errors = domainResult.Errors
		});
	}
}