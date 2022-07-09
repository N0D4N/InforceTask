using InforceTask.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InforceTask.DataAccess.DbMigrator;

public static class DbMigrator
{
	public static async Task MigrateDbAsync(IServiceProvider serviceProvider)
	{
		using (var serviceScope = serviceProvider.CreateScope())
		{
			var appDbContext = serviceScope.ServiceProvider.GetService<AppDbContext>()!;
			await appDbContext.Database.EnsureCreatedAsync();
			await appDbContext.Database.MigrateAsync();
			if (!await appDbContext.AboutPageContents.AnyAsync())
			{
				await appDbContext.AboutPageContents.AddAsync(new AboutPageContents
				{
					Content =
						@"Unfortunately i didn't have enough time to finish task completely. Admin still can't edit about page content, though it's dynamically taken from server
All sync and not async calls to DbContext are intended since SQLite (provider for this task) doesn't support async and it would only result in worse performance.
Algo for generating shortened url is pretty simple: create new guid, get a slice of it's string representation, ensure that there isn't alredy such shortcut in DB and you're done.",
					LastEditDateTime = DateTime.UtcNow
				});
				await appDbContext.SaveChangesAsync();
			}
		}
	}
}