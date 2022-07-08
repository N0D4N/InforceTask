namespace InforceTask.Models;

public sealed class ShortenedUrlInfoResult : DomainResult<ShortenedUrl>
{
	public static ShortenedUrlInfoResult FromFailure(string error)
	{
		return FromFailures(new[] { error });
	}

	public static ShortenedUrlInfoResult FromFailures(IReadOnlyCollection<string> errors) => new()
	{
		Errors = errors
	};
}