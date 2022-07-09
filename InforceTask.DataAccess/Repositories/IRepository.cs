namespace InforceTask.DataAccess.Repositories;

public interface IRepository : IDisposable
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}