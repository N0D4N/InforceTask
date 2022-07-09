using InforceTask.Contracts.Requests;

namespace InforceTask.Domain.Services;

public interface IAboutRedactorService
{
	Task<AboutPageContents> ChangeAboutPageContentsAsync(EditAboutPageRequest request, CancellationToken cancellationToken = default);
	Task<AboutPageContents> GetCurrentAboutPageContentsAsync(CancellationToken cancellationToken = default);
}