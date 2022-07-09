using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InforceTask.Domain;

public sealed class RefreshToken
{
	[Key]
	public string Token { get; set; }

	public string JwtId { get; set; }

	public DateTime CreationDate { get; set; }

	public DateTime ExpiryDate { get; set; }
	public Guid UserId { get; init; }

	[ForeignKey(nameof(UserId))]
	public User User { get; init; }
}