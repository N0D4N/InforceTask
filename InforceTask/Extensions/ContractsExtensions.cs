using InforceTask.Contracts.Responses;
using InforceTask.Domain;

namespace InforceTask.Extensions;

public static class ContractsExtensions
{
	public static ShortenedUrlResponse ToShortenedUrlResponse(this ShortenedUrl shortenedUrl)
	{
		return new ShortenedUrlResponse
		{
			Creator = shortenedUrl.Creator?.ToUserResponse(),
			Destination = shortenedUrl.Destination,
			Id = shortenedUrl.Id,
			CreationDateUnixTimestampInSeconds = shortenedUrl.CreationDateUnixTimestampInSeconds
		};
	}

	public static UserResponse ToUserResponse(this User user)
	{
		return new UserResponse()
		{
			Id = user.Id,
			Username = user.Username
		};
	}

	public static TokensPairResponse ToTokensPairResponse(this TokensPair tokensPair)
	{
		return new TokensPairResponse()
		{
			AccessToken = tokensPair.AccessToken,
			RefreshToken = tokensPair.RefreshToken
		};
	}
}