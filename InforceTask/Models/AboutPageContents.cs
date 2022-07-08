using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InforceTask.Models;

public sealed class AboutPageContents
{
	
	public static readonly AboutPageContents Default = new AboutPageContents()
	{
		Content =
			@"Unfortunately i didn't have enough time to finish task completely. Admin still can't edit about page content, though it's dynamically taken from server
All sync and not async calls to DbContext are intended since SQLite (provider for this task) doesn't support async and it would only result in worse performance.
Algo for generating shortened url is pretty simple: create new guid, get a slice of it's string representation, ensure that there isn't alredy such shortcut in DB and you're done.",
		LastEditDateTime = DateTime.UtcNow,
		EditNumber = 1
	};
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public uint EditNumber { get; set; }

	public string Content { get; init; }
	public DateTime LastEditDateTime { get; init; }
}