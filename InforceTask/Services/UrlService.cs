using InforceTask.Data;
using InforceTask.Models;
using Microsoft.EntityFrameworkCore;

namespace InforceTask.Services;

public sealed class UrlService : IUrlService
{
	private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

	public IReadOnlyCollection<ShortenedUrl> GetAll()
	{
		using var dbContext = this._dbContextFactory.CreateDbContext();
		return dbContext.ShortenedUrls.Include(x => x.Creator).AsNoTracking().ToList();
	}

	public ShortenedUrlDeletionResult DeleteUrl(string id, Guid userId)
	{
		using var dbContext = this._dbContextFactory.CreateDbContext();
		var urlToDelete = dbContext.ShortenedUrls.Include(x => x.Creator).FirstOrDefault(x => x.Id == id);
		if (urlToDelete is null)
		{
			return ShortenedUrlDeletionResult.FromFailure("Couldn't find shortened url with such id");
		}

		if (!urlToDelete.CreatorId.Equals(userId) && dbContext.Users.FirstOrDefault(x => x.Id == userId)?.IsAdmin != true)
		{
			return ShortenedUrlDeletionResult.FromFailure("You don't have enough permissions to delete this shortened url");
		}

		dbContext.ShortenedUrls.Remove(urlToDelete);
		dbContext.SaveChanges();
		return ShortenedUrlDeletionResult.SuccessfulDeletion;
	}

	public ShortenedUrlInfoResult GetShortUrl(string id)
	{
		using var dbContext = this._dbContextFactory.CreateDbContext();
		var shortenedUrl = dbContext.ShortenedUrls.Include(x => x.Creator).FirstOrDefault(x => x.Id == id);
		if (shortenedUrl is null)
		{
			return ShortenedUrlInfoResult.FromFailure("Couldn't find short url with such id");
		}

		return new()
		{
			Data = shortenedUrl
		};
	}

	public ShortenedUrlCreationResult CreateShortenedUrl(string destination, Guid creatorId)
	{
		using var dbContext = this._dbContextFactory.CreateDbContext();
		if (dbContext.ShortenedUrls.Any(x => x.Destination == destination))
		{
			return ShortenedUrlCreationResult.FromFailure("Url pointing to the same destination already exist");
		}

		Span<char> buffer = stackalloc char[32];
		var id = "";
		var isIdUnique = false;
		while (!isIdUnique)
		{
			var guid = Guid.NewGuid();
			guid.TryFormat(buffer, out var _, "N");
			for (var i = 6; i <= 32; i++)
			{
				id = buffer.Slice(0, i).ToString();
				if (dbContext.ShortenedUrls.Any(x => x.Id == id))
				{
					continue;
				}

				isIdUnique = true;
				break;
			}
		}

		var shortenedUrl = new ShortenedUrl
		{
			Destination = destination,
			Id = id,
			CreationDateUnixTimestampInSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
			CreatorId = creatorId
		};
		dbContext.ShortenedUrls.Add(shortenedUrl);
		dbContext.SaveChanges();
		return new()
		{
			Data = shortenedUrl
		};
	}

	public UrlService(IDbContextFactory<AppDbContext> dbContextFactory)
	{
		this._dbContextFactory = dbContextFactory;
	}
}