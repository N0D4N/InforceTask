using InforceTask.Domain;
using Microsoft.EntityFrameworkCore;

namespace InforceTask.DataAccess.Repositories;

internal sealed class AboutPageRepository : BaseRepository, IAboutPageRepository
{
	public AboutPageRepository(AppDbContext appDbContext) : base(appDbContext)
	{ }

	public async Task ChangeContentsTo(AboutPageContents aboutPageContents, CancellationToken cancellationToken = default)
	{
		await this._appDbContext.AboutPageContents.AddAsync(aboutPageContents, cancellationToken);
	}

	public Task<AboutPageContents> GetCurrentAboutPage(CancellationToken cancellationToken = default)
	{
		return this._appDbContext.AboutPageContents.AsNoTracking().OrderByDescending(x => x.EditNumber).FirstOrDefaultAsync(cancellationToken)!;
	}
}