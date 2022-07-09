using InforceTask.Domain;
using Microsoft.EntityFrameworkCore;

namespace InforceTask.DataAccess.Repositories;

internal sealed class ShortenedUrlsRepository : BaseRepository, IShortenedUrlRepository
{
	public ShortenedUrlsRepository(AppDbContext appDbContext) : base(appDbContext)
	{ }

	public async Task<IReadOnlyCollection<ShortenedUrl>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await this._appDbContext.ShortenedUrls.Include(x => x.Creator).AsNoTracking().ToListAsync(cancellationToken);
	}

	public Task<ShortenedUrl?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		return this._appDbContext.ShortenedUrls.Include(x => x.Creator).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

	public async Task RemoveAsync(ShortenedUrl shortenedUrl, CancellationToken cancellationToken = default)
	{
		this._appDbContext.ShortenedUrls.Remove(shortenedUrl);
		await this._appDbContext.SaveChangesAsync(cancellationToken);
	}

	public async  Task CreateShortenedUrl(ShortenedUrl shortenedUrl, CancellationToken cancellationToken = default)
	{
		await this._appDbContext.ShortenedUrls.AddAsync(shortenedUrl, cancellationToken);
	}

	public Task<bool> HasShortenedUrlWithDestination(string destination, CancellationToken cancellationToken = default)
	{
		return this._appDbContext.ShortenedUrls.AnyAsync(x => x.Destination == destination, cancellationToken);
	}

	public Task<bool> Exists(string id, CancellationToken cancellationToken = default)
	{
		return this._appDbContext.ShortenedUrls.AnyAsync(x => x.Id == id, cancellationToken);
	}
}