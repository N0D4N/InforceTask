using InforceTask.Domain;
using Microsoft.EntityFrameworkCore;

namespace InforceTask.DataAccess;

internal sealed class AppDbContext : DbContext
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
	}
}