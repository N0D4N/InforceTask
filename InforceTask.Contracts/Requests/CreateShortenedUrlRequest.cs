using System.ComponentModel.DataAnnotations;

namespace InforceTask.Contracts.Requests;

public sealed class CreateShortenedUrlRequest
{
	[Url]
	[Required(ErrorMessage = $"{nameof(Destination)} is required")]
	public string Destination { get; init; }
}