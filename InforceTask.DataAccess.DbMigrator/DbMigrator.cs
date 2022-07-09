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
						@"First about page content. Click to edit it while logged in as admin user.
Algo for generating shortened url is pretty simple: create new guid, get a slice of it's string representation, ensure that there isn't alredy such shortcut in DB and you're done.",
					LastEditDateTime = DateTime.UtcNow
				});
				await appDbContext.SaveChangesAsync();
			}
		}
	}
}
