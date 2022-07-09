using Microsoft.Extensions.DependencyInjection;

namespace InforceTask.Domain.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDomainServices(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddScoped<IUserService, UserService>();
		serviceCollection.AddScoped<IUrlService, UrlService>();
		serviceCollection.AddScoped<IAboutRedactorService, AboutRedactorService>();
		return serviceCollection;
	}
}