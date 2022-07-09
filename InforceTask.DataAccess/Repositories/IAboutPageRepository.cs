using InforceTask.Domain;

namespace InforceTask.DataAccess.Repositories;

public interface IAboutPageRepository : IRepository
{
	Task ChangeContentsTo(AboutPageContents aboutPageContents, CancellationToken cancellationToken = default);
	Task<AboutPageContents> GetCurrentAboutPage(CancellationToken cancellationToken = default);
}