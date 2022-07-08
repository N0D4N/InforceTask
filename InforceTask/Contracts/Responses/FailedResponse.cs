namespace InforceTask.Contracts.Responses;

public class FailedResponse
{
	public IReadOnlyCollection<string> Errors { get; init; }
}