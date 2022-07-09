namespace InforceTask.DataAccess.Repositories;

internal abstract class BaseRepository : IRepository
{
	protected readonly AppDbContext _appDbContext;

	protected BaseRepository(AppDbContext appDbContext)
	{
		this._appDbContext = appDbContext;
	}

	public void Dispose()
	{
		this._appDbContext.Dispose();
	}

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => this._appDbContext.SaveChangesAsync(cancellationToken);
}