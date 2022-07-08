namespace InforceTask.Models;

public sealed class ShortenedUrlCreationResult : DomainResult<ShortenedUrl>
{
	public static ShortenedUrlCreationResult FromFailures(IReadOnlyCollection<string>? errors) => new()
	{
		Errors = errors
	};

	public static ShortenedUrlCreationResult FromFailure(string error)
	{
		return FromFailures(new[] { error });
	}
}