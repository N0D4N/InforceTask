using InforceTask.Contracts.Requests;
using InforceTask.Contracts.Responses;
using InforceTask.Extensions;
using InforceTask.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InforceTask.Controllers;

public sealed class UrlsController : InforceBaseApiController
{
	private readonly IUrlService _urlService;

	public UrlsController(IUrlService urlService)
	{
		this._urlService = urlService;
	}

	[HttpGet(Constants.Routes.Urls.All)]
	[ProducesResponseType(typeof(AllShortenedUrlsResponse), 200)]
	public IActionResult GetAllUrls()
	{
		var shortenedUrls = this._urlService.GetAll();
		return this.Ok(new AllShortenedUrlsResponse
		{
			Data = shortenedUrls.Select(x => new ShortenedUrlResponse(x)).ToArray()
		});
	}

	[HttpPost(Constants.Routes.Urls.Url)]
	[Authorize]
	[ProducesResponseType(typeof(CreateShortenedUrlResponse), 200)]
	[ProducesErrorResponseType(typeof(FailedResponse))]
	public IActionResult CreateUrl([FromBody] CreateShortenedUrlRequest request)
	{
		var userId = this.HttpContext.GetUserId();
		if (userId is null)
		{
			return this.Unauthorized();
		}

		var shortenedUrlCreationResult = this._urlService.CreateShortenedUrl(request.Destination, userId.Value);
		if (shortenedUrlCreationResult.Success)
		{
			return this.Ok(new CreateShortenedUrlResponse
			{
				Data = new(shortenedUrlCreationResult.Data)
			});
		}

		return this.BadRequest(new FailedResponse
		{
			Errors = shortenedUrlCreationResult.Errors
		});
	}

	[HttpDelete(Constants.Routes.Urls.UrlWithIdentifier)]
	[Authorize]
	[ProducesResponseType(typeof(OkResult), 200)]
	[ProducesErrorResponseType(typeof(FailedResponse))]
	public IActionResult DeleteUrl(string Id)
	{
		var userId = this.HttpContext.GetUserId();
		if (userId is null)
		{
			return this.Unauthorized();
		}

		var shortenedUrlDeletionResult = this._urlService.DeleteUrl(Id, userId.Value);
		return this.MapDomainResult(shortenedUrlDeletionResult);
	}

	[HttpGet(Constants.Routes.Urls.UrlWithIdentifier)]
	[ProducesResponseType(typeof(ShortenedUrlInfoResponse), 200)]
	[ProducesErrorResponseType(typeof(FailedResponse))]
	public IActionResult GetUrlInfo(string Id)
	{
		var shortenedUrl = this._urlService.GetShortUrl(Id);
		if (shortenedUrl.Success)
		{
			return this.Ok(new ShortenedUrlInfoResponse
			{
				Data = new(shortenedUrl.Data)
			});
		}

		return this.BadRequest(new FailedResponse
		{
			Errors = shortenedUrl.Errors
		});
	}
}