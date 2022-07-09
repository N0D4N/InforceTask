namespace InforceTask.Domain;

public static class ClaimTypes
{
	public const string Id = System.Security.Claims.ClaimTypes.NameIdentifier;

	public const string Username = "Username";

	public const string Admin = "Admin";

	public const string JwtId = "jti";

	public const string JwtSubject = "sub";

	public const string JwtExpiration = "exp";
}
