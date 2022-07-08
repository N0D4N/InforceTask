namespace InforceTask.Contracts.Responses;

public sealed class AboutPageContentsResponse
{
	public uint EditNumber { get; init; }
	public string Content { get; init; }
	public DateTime LastEditDateTime { get; init; }
}