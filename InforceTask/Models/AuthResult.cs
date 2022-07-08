namespace InforceTask.Models;

public sealed class AuthResult : DomainResult<TokensPair>
{
	public static AuthResult FromFailures(IReadOnlyCollection<string>? errors) => new()
	{
		Errors = errors
	};

	public static AuthResult FromFailure(string error)
	{
		return FromFailures(new[] { error });
	}
}