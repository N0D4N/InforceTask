namespace InforceTask.Models;

public abstract class DomainResult<T> : DomainResult
{
	public T Data { get; init; }
}

public abstract class DomainResult
{
	public IReadOnlyCollection<string>? Errors { get; protected init; }
	public bool Success => this.Errors is null;
}