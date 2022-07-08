using InforceTask.Models;

namespace InforceTask.Services;

public interface IUrlService
{
	IReadOnlyCollection<ShortenedUrl> GetAll();
	ShortenedUrlDeletionResult DeleteUrl(string id, Guid userId);
	ShortenedUrlInfoResult? GetShortUrl(string id);
	ShortenedUrlCreationResult CreateShortenedUrl(string destination, Guid creatorId);
}