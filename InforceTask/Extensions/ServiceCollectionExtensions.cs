using System.Reflection;
using InforceTask.Domain.Services;
using InforceTask.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace InforceTask.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAuthServices(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
		serviceCollection.AddSingleton(jwtOptions);
		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = jwtOptions.GetSecurityKey(),
			ValidateIssuer = false,
			ValidateAudience = false,
			RequireExpirationTime = false,
			ValidateLifetime = true
		};

		serviceCollection.AddSingleton(tokenValidationParameters);
		serviceCollection.AddAuthentication(x =>
		{
			x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(x =>
		{
			x.SaveToken = true;
			x.TokenValidationParameters = tokenValidationParameters;
		});
		serviceCollection.AddAuthorization();
		return serviceCollection;
	}

	public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddSwaggerGen(x =>
		{
			x.SwaggerDoc("InforceTask", new()
			{
				Title = "InforceTask"
			});
			x.AddSecurityDefinition("Bearer", new()
			{
				Description = "Jwt bearer auth",
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey
			});
			x.AddSecurityRequirement(new()
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new()
						{
							Id = "Bearer",
							Type = ReferenceType.SecurityScheme
						}
					},
					new List<string>()
				}
			});
			x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
		});
		return serviceCollection;
	}
}