using InforceTask.Domain;

namespace InforceTask.DataAccess.Repositories;

public interface IRefreshTokenRepository : IRepository
{
	Task AddTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
	Task<RefreshToken?> GetRefreshTokenByValueAsync(string refreshToken, CancellationToken cancellationToken = default);
}