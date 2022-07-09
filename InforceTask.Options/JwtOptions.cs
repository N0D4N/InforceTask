namespace InforceTask.Options;

public sealed class JwtOptions
{

	public string Secret { get; init; }

	public TimeSpan AccessTokenLifetime { get; init; }

	public TimeSpan RefreshTokenLifetime { get; init; }
}