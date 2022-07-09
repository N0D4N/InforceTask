using System.ComponentModel.DataAnnotations;

namespace InforceTask.Contracts.Requests;

public sealed class LoginRequest
{
	[Required(ErrorMessage = $"{nameof(Username)} is required")]
	public string Username { get; init; }

	[Required(ErrorMessage = $"{nameof(Password)} is required")]
	public string Password { get; init; }

	[Required(ErrorMessage = $"{nameof(ShouldBeAdmin)} should be set")]
	public bool ShouldBeAdmin { get; init; }
}