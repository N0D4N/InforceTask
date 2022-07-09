using InforceTask.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InforceTask.DataAccess;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddScoped(typeof(IRepositoryFactory<>), typeof(RepositoryFactory<>));
		serviceCollection.AddScoped<IAboutPageRepository, AboutPageRepository>();
		serviceCollection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
		serviceCollection.AddScoped<IShortenedUrlRepository, ShortenedUrlsRepository>();
		serviceCollection.AddScoped<IUserRepository, UserRepository>();
		serviceCollection.AddDbContextFactory<AppDbContext>((services, options) =>
			options.UseSqlServer(services.GetService<IConfiguration>().GetConnectionString("SqlServer")));
		return serviceCollection;
	}
}