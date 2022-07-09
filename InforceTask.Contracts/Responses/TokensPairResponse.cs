namespace InforceTask.Contracts.Responses;

public sealed class TokensPairResponse
{
	public string AccessToken { get; init; }
	public string RefreshToken { get; init; }
}