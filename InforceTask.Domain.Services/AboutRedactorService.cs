using InforceTask.Contracts.Requests;
using InforceTask.DataAccess.Repositories;

namespace InforceTask.Domain.Services;

internal sealed class AboutRedactorService : IAboutRedactorService
{
	private readonly IAboutPageRepository _aboutPageRepository;

	public async Task<AboutPageContents> ChangeAboutPageContentsAsync(EditAboutPageRequest request, CancellationToken cancellationToken = default)
	{
		var currentAboutPage = await this._aboutPageRepository.GetCurrentAboutPage(cancellationToken);
		if (currentAboutPage.Content != request.Content)
		{
			var newAboutPage = new AboutPageContents
			{
				Content = request.Content,
				LastEditDateTime = DateTime.UtcNow
			};
			await this._aboutPageRepository.ChangeContentsTo(newAboutPage, cancellationToken);
			await this._aboutPageRepository.SaveChangesAsync(cancellationToken);
			return await this._aboutPageRepository.GetCurrentAboutPage(cancellationToken);
		}
		else
		{
			return currentAboutPage;
		}
	}

	public AboutRedactorService(IAboutPageRepository aboutPageRepository)
	{
		this._aboutPageRepository = aboutPageRepository;
	}

	public Task<AboutPageContents> GetCurrentAboutPageContentsAsync(CancellationToken cancellationToken = default)
	{
		return this._aboutPageRepository.GetCurrentAboutPage(cancellationToken);
	}
}