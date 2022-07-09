using System.Reflection;
using InforceTask.Contracts.Responses;
using InforceTask.DataAccess;
using InforceTask.Domain.Services;
using InforceTask.Extensions;
using InforceTask.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace InforceTask;

public static class Startup
{
	public static void ConfigureServices(WebApplicationBuilder builder)
	{
		builder.Services.AddDataAccess();
		builder.Services.AddDomainServices();
		builder.Services.AddAuthServices(builder.Configuration);
		builder.Services.AddSwagger();
		builder.Services.AddControllersWithViews().ConfigureApiBehaviorOptions(options =>
		{
			options.InvalidModelStateResponseFactory = context =>
			{
				return new BadRequestObjectResult(new FailedResponse()
				{
					Errors = context.ModelState.Values.SelectMany(x => x.Errors).Select(x=>x.ErrorMessage).ToArray()
				});
			};
		});
	}

	public static void Configure(WebApplication app)
	{
		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		//app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();
		var swaggerOptions = app.Configuration.GetSection(nameof(SwaggerOptions)).Get<SwaggerOptions>();
		app.UseSwagger(x => x.RouteTemplate = swaggerOptions.JsonRoute);
		app.UseSwaggerUI(x => x.SwaggerEndpoint(swaggerOptions.UiEndpoint, swaggerOptions.Description));
		app.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");
		app.MapFallbackToFile("index.html");
	}
}