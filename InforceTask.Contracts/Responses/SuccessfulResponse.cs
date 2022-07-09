namespace InforceTask.Contracts.Responses;

public abstract class SuccessfulResponse<T>
{
	public T Data { get; init; }
}