using System.ComponentModel.DataAnnotations.Schema;

namespace InforceTask.Models;

public sealed class ShortenedUrl
{
	public string Id { get; init; }

	public string Destination { get; init; }

	public long CreationDateUnixTimestampInSeconds { get; init; }

	public Guid CreatorId { get; init; }

	[ForeignKey(nameof(CreatorId))]
	public User Creator { get; init; }
}