namespace InforceTask.Domain.Services;

public interface IUrlService
{
	Task<IReadOnlyCollection<ShortenedUrl>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<ShortenedUrlDeletionResult> DeleteUrlAsync(string id, Guid userId, CancellationToken cancellationToken = default);
	Task<ShortenedUrlInfoResult> GetShortUrlAsync(string id, CancellationToken cancellationToken = default);
	Task<ShortenedUrlCreationResult> CreateShortenedUrlAsync(string destination, Guid creatorId,CancellationToken cancellationToken = default);
}