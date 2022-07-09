using Microsoft.Extensions.DependencyInjection;

namespace InforceTask.DataAccess.Repositories;

internal sealed class RepositoryFactory<TRepository> : IRepositoryFactory<TRepository> where TRepository : IRepository
{
	private readonly IServiceProvider _serviceProvider;

	public RepositoryFactory(IServiceProvider serviceProvider)
	{
		this._serviceProvider = serviceProvider;
	}

	public TRepository CreateRepository()
	{
		return this._serviceProvider.GetService<TRepository>()!;
	}
}