using System.Reflection;
using InforceTask.Data;
using InforceTask.Options;
using InforceTask.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContextFactory<AppDbContext>((services, options) =>
{
	options.UseSqlite(services.GetRequiredService<IConfiguration>().GetConnectionString("Sqlite"));
});
var jwtOptions = builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IUrlService, UrlService>();
builder.Services.AddSingleton<IAboutRedactorService, AboutRedactorService>();
var tokenValidationParameters = new TokenValidationParameters
{
	ValidateIssuerSigningKey = true,
	IssuerSigningKey = jwtOptions.Key,
	ValidateIssuer = false,
	ValidateAudience = false,
	RequireExpirationTime = false,
	ValidateLifetime = true
};

builder.Services.AddSingleton(tokenValidationParameters);
builder.Services.AddAuthentication(x =>
{
	x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
	x.SaveToken = true;
	x.TokenValidationParameters = tokenValidationParameters;
});
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(x =>
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
builder.Services.AddControllersWithViews();

var app = builder.Build();

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

using (var appDbContext = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext())
{
	appDbContext.Database.EnsureCreated();
	appDbContext.Database.Migrate();
}

app.Run();