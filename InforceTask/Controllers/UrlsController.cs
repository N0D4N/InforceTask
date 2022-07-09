using InforceTask.Contracts.Requests;
using InforceTask.Contracts.Responses;
using InforceTask.Domain.Services;
using InforceTask.Extensions;
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
	public async Task<IActionResult> GetAllUrlsAsync()
	{
		var shortenedUrls = await this._urlService.GetAllAsync();
		return this.Ok(new AllShortenedUrlsResponse
		{
			Data = shortenedUrls.Select(x => x.ToShortenedUrlResponse()).ToArray()
		});
	}

	[HttpPost(Constants.Routes.Urls.Url)]
	[Authorize]
	[ProducesResponseType(typeof(CreateShortenedUrlResponse), 200)]
	[ProducesErrorResponseType(typeof(FailedResponse))]
	public async Task<IActionResult> CreateUrlAsync([FromBody] CreateShortenedUrlRequest request)
	{
		var userId = this.HttpContext.GetUserId();
		if (userId is null)
		{
			return this.Unauthorized();
		}

		var shortenedUrlCreationResult = await this._urlService.CreateShortenedUrlAsync(request.Destination, userId.Value);
		if (shortenedUrlCreationResult.Success)
		{
			return this.Ok(new CreateShortenedUrlResponse
			{
				Data = shortenedUrlCreationResult.Data.ToShortenedUrlResponse()
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
	public async Task<IActionResult> DeleteUrlAsync(string id)
	{
		var userId = this.HttpContext.GetUserId();
		if (userId is null)
		{
			return this.Unauthorized();
		}

		var shortenedUrlDeletionResult = await this._urlService.DeleteUrlAsync(id, userId.Value);
		return this.MapDomainResult(shortenedUrlDeletionResult);
	}

	[HttpGet(Constants.Routes.Urls.UrlWithIdentifier)]
	[ProducesResponseType(typeof(ShortenedUrlInfoResponse), 200)]
	[ProducesErrorResponseType(typeof(FailedResponse))]
	public async Task<IActionResult> GetUrlInfoAsync(string id)
	{
		var shortenedUrl = await this._urlService.GetShortUrlAsync(id);
		if (shortenedUrl.Success)
		{
			return this.Ok(new ShortenedUrlInfoResponse
			{
				Data = shortenedUrl.Data.ToShortenedUrlResponse()
			});
		}

		return this.BadRequest(new FailedResponse
		{
			Errors = shortenedUrl.Errors
		});
	}
}