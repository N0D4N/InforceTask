using InforceTask.Domain;
using Microsoft.EntityFrameworkCore;

namespace InforceTask.DataAccess.Repositories;

internal sealed class RefreshTokenRepository : BaseRepository, IRefreshTokenRepository
{

	public RefreshTokenRepository(AppDbContext appDbContext) : base(appDbContext)
	{
	}

	public async Task AddTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
	{
		await this._appDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
	}

	public Task<RefreshToken?> GetRefreshTokenByValueAsync(string refreshToken, CancellationToken cancellationToken = default)
	{
		return this._appDbContext.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);
	}
}