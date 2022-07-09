using InforceTask.Domain;

namespace InforceTask.DataAccess.Repositories;

public interface IShortenedUrlRepository : IRepository
{
	Task<IReadOnlyCollection<ShortenedUrl>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<ShortenedUrl?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
	Task RemoveAsync(ShortenedUrl shortenedUrl, CancellationToken cancellationToken = default);
	Task CreateShortenedUrl(ShortenedUrl shortenedUrl, CancellationToken cancellationToken = default);
	Task<bool> HasShortenedUrlWithDestination(string destination, CancellationToken cancellationToken = default);
	Task<bool> Exists(string id, CancellationToken cancellationToken = default);
}