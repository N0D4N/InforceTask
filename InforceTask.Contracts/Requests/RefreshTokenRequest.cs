using System.ComponentModel.DataAnnotations;

namespace InforceTask.Contracts.Requests;

public sealed class RefreshTokenRequest
{
	[Required(ErrorMessage = $"{nameof(AccessToken)} is required")]

	public string AccessToken { get; init; }

	[Required(ErrorMessage = $"{nameof(RefreshToken)} is required")]

	public string RefreshToken { get; init; }
}