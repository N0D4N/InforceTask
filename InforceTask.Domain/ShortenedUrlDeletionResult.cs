namespace InforceTask.Domain;

public sealed class ShortenedUrlDeletionResult : DomainResult
{
	public static readonly ShortenedUrlDeletionResult SuccessfulDeletion = new();

	public static ShortenedUrlDeletionResult FromFailure(string error)
	{
		return FromFailures(new[] { error });
	}

	private static ShortenedUrlDeletionResult FromFailures(IReadOnlyCollection<string> errors) => new()
	{
		Errors = errors
	};
}