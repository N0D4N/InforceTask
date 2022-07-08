using System.ComponentModel.DataAnnotations;

namespace InforceTask.Models;

public sealed class User
{
	[Key]
	public Guid Id { get; init; }

	public string Username { get; init; }
	public string PasswordHash { get; init; }
	public string PasswordSalt { get; init; }
	public IReadOnlyCollection<ShortenedUrl>? Urls { get; init; }
	public bool IsAdmin { get; init; }
}