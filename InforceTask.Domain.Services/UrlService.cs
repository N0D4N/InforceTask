using InforceTask.DataAccess.Repositories;

namespace InforceTask.Domain.Services;

internal sealed class UrlService : IUrlService
{
	private readonly IShortenedUrlRepository _shortenedUrlRepository;
	private readonly IUserRepository _userRepository;

	public Task<IReadOnlyCollection<ShortenedUrl>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var shortenedUrlRepository = this._shortenedUrlRepository;
		return shortenedUrlRepository.GetAllAsync(cancellationToken);
	}

	public async Task<ShortenedUrlDeletionResult> DeleteUrlAsync(string id, Guid userId, CancellationToken cancellationToken = default)
	{
		var urlToDelete = await this._shortenedUrlRepository.GetByIdAsync(id, cancellationToken);
		if (urlToDelete is null)
		{
			return ShortenedUrlDeletionResult.FromFailure("Couldn't find shortened url with such id");
		}

		if (!urlToDelete.CreatorId.Equals(userId) && await this._userRepository.IsUserAdminAsync(userId, cancellationToken))
		{
			return ShortenedUrlDeletionResult.FromFailure("You don't have enough permissions to delete this shortened url");
		}

		await this._shortenedUrlRepository.RemoveAsync(urlToDelete, cancellationToken);
		return ShortenedUrlDeletionResult.SuccessfulDeletion;
	}

	public async Task<ShortenedUrlInfoResult> GetShortUrlAsync(string id, CancellationToken cancellationToken = default)
	{
		var shortenedUrl = await this._shortenedUrlRepository.GetByIdAsync(id, cancellationToken);
		if (shortenedUrl is null)
		{
			return ShortenedUrlInfoResult.FromFailure("Couldn't find short url with such id");
		}

		return new()
		{
			Data = shortenedUrl
		};
	}

	public async Task<ShortenedUrlCreationResult> CreateShortenedUrlAsync(string destination, Guid creatorId,CancellationToken cancellationToken = default)
	{
		if (await this._shortenedUrlRepository.HasShortenedUrlWithDestination(destination))
		{
			return ShortenedUrlCreationResult.FromFailure("Url pointing to the same destination already exist");
		}

		var buffer = new char[32];
		var id = "";
		var isIdUnique = false;
		while (!isIdUnique)
		{
			var guid = Guid.NewGuid();
			guid.TryFormat(buffer, out var _, "N");
			for (var i = 6; i <= 32; i++)
			{
				id = buffer.AsSpan().Slice(0, i).ToString();
				if (await this._shortenedUrlRepository.Exists(id, cancellationToken))
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
		await this._shortenedUrlRepository.CreateShortenedUrl(shortenedUrl, cancellationToken);
		await this._shortenedUrlRepository.SaveChangesAsync(cancellationToken);
		return new()
		{
			Data = shortenedUrl
		};
	}

	public UrlService(IShortenedUrlRepository shortenedUrlRepository,
					  IUserRepository userRepository)
	{
		this._shortenedUrlRepository = shortenedUrlRepository;
		this._userRepository = userRepository;
	}
}