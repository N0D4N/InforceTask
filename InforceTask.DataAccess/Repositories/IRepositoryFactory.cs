namespace InforceTask.DataAccess.Repositories;

public interface IRepositoryFactory<TRepository> where TRepository: IRepository
{
	TRepository CreateRepository();
}