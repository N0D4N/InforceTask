namespace InforceTask.Domain;

public sealed class TokensPair
{
	public string AccessToken { get; init; }
	public string RefreshToken { get; init; }
}