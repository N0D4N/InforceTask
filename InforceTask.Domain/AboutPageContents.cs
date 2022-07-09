using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InforceTask.Domain;

public sealed class AboutPageContents
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public uint EditNumber { get; set; }

	public string Content { get; init; }
	public DateTime LastEditDateTime { get; init; }
}