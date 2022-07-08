using InforceTask.Data;
using InforceTask.Models;
using Microsoft.EntityFrameworkCore;

namespace InforceTask.Services;

public sealed class AboutRedactorService : IAboutRedactorService
{
	private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

	public AboutPageContents ChangeAboutPageContentsTo(string content)
	{
		var aboutPageContents = new AboutPageContents
		{
			Content = content,
			LastEditDateTime = DateTime.UtcNow
		};
		using var dbContext = this._dbContextFactory.CreateDbContext();
		dbContext.AboutPageContents.Add(aboutPageContents);
		return dbContext.AboutPageContents.OrderByDescending(x => x.EditNumber).First();
	}

	public AboutRedactorService(IDbContextFactory<AppDbContext> dbContextFactory)
	{
		this._dbContextFactory = dbContextFactory;
	}

	public AboutPageContents GetCurrentAboutPageContents()
	{
		using var dbContext = this._dbContextFactory.CreateDbContext();
		return dbContext.AboutPageContents.OrderByDescending(x => x.EditNumber).FirstOrDefault() ?? AboutPageContents.Default;
	}
}