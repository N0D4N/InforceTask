using InforceTask.Models;

namespace InforceTask.Contracts.Responses;

public class ShortenedUrlResponse
{
	public string Id { get; init; }

	public string Destination { get; init; }

	public long CreationDateUnixTimestampInSeconds { get; init; }

	public UserResponse? Creator { get; init; }

	public ShortenedUrlResponse(ShortenedUrl shortenedUrl)
	{
		this.Id = shortenedUrl.Id;
		this.Destination = shortenedUrl.Destination;
		this.CreationDateUnixTimestampInSeconds = shortenedUrl.CreationDateUnixTimestampInSeconds;
		if (shortenedUrl.Creator is not null)
		{
			this.Creator = new(shortenedUrl.Creator);
		}
	}
}