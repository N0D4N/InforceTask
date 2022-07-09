namespace InforceTask.Contracts.Responses;

public class ShortenedUrlResponse
{
	public string Id { get; init; }

	public string Destination { get; init; }

	public long CreationDateUnixTimestampInSeconds { get; init; }

	public UserResponse? Creator { get; init; }
}