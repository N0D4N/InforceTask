using InforceTask.Models;
using Microsoft.EntityFrameworkCore;

namespace InforceTask.Data;

public sealed class AppDbContext : DbContext
{
	public DbSet<User> Users { get; init; }
	public DbSet<ShortenedUrl> ShortenedUrls { get; init; }

	public DbSet<RefreshToken> RefreshTokens { get; init; }

	public DbSet<AboutPageContents> AboutPageContents { get; init; }

	public AppDbContext(DbContextOptions options) : base(options)
	{ }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<ShortenedUrl>().HasOne(u => u.Creator).WithMany(u => u.Urls);
		modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();
		modelBuilder.Entity<ShortenedUrl>().HasKey(x => new
		{
			x.Id,
			x.Destination
		});
		modelBuilder.Entity<AboutPageContents>().HasData(new AboutPageContents
		{
			Content =
				@"Unfortunately i didn't have enough time to finish task completely. Admin still can't edit about page content, though it's dynamically taken from server
All sync and not async calls to DbContext are intended since SQLite (provider for this task) doesn't support async and it would only result in worse performance.
Algo for generating shortened url is pretty simple: create new guid, get a slice of it's string representation, ensure that there isn't alredy such shortcut in DB and you're done.",
			LastEditDateTime = DateTime.UtcNow,
			EditNumber = 1
		});
	}
}