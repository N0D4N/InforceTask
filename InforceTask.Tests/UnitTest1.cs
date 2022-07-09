using System.Text;
using System.Text.Json;
using InforceTask.Contracts.Requests;
using InforceTask.DataAccess;
using InforceTask.DataAccess.Repositories;
using InforceTask.Domain;
using InforceTask.Domain.Services;
using InforceTask.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InforceTask.Tests;

public class UnitTest1
{
	
	private AppDbContext Context => new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase($"Testing{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}").Options);
	[Fact]
	public async Task AboutPageRepoShouldReturnMostRecentEntry()
	{
		var repo = new AboutPageRepository(Context);
		for (uint i = 1; i < 11; i++)
		{
			await repo.ChangeContentsTo(new AboutPageContents()
			{
				Content = i.ToString(),
				LastEditDateTime = DateTime.UtcNow
			});
		}

		await repo.SaveChangesAsync();
		var aboutPageContents = await repo.GetCurrentAboutPage();
		Assert.Equal(10u, aboutPageContents.EditNumber);
		Assert.Equal(10.ToString(), aboutPageContents.Content);
	}

	[Fact]
	public async Task UserShouldNotEnterWithInvalidPassword()
	{
		var jwtOptions = new JwtOptions()
		{
			Secret = "c2VjcmV0bHRyYWxvbmdrZXlxd3F3ZXFlcXdxd2Vxc2RhZHF3MzIxMw==",
			AccessTokenLifetime = TimeSpan.FromMinutes(1),
			RefreshTokenLifetime = TimeSpan.FromMinutes(5)
		};
		var userService = new UserService(new UserRepository(this.Context), new RefreshTokenRepository(this.Context), jwtOptions, new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = jwtOptions.GetSecurityKey(),
			ValidateIssuer = false,
			ValidateAudience = false,
			RequireExpirationTime = false,
			ValidateLifetime = true
		});
		var initialLoginRequest = new LoginRequest()
		{
			Password = "Pass0",
			ShouldBeAdmin = false,
			Username = "user"
		};
		var authResultRegister = await userService.RegisterUserAsync(initialLoginRequest);
		Assert.True(authResultRegister.Success);
		var base64content = authResultRegister.Data.AccessToken.Split('.')[1] + "==";
		var jsonDocument = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(base64content)));
		var id = Guid.Parse(jsonDocument.RootElement.GetProperty("sub").GetString());
		var authResultLogin = await userService.LoginUserAsync(initialLoginRequest, id);
		Assert.True(authResultLogin.Success);
		var loginRequestWithInvalidPasword = new LoginRequest()
		{
			Username = initialLoginRequest.Username,
			ShouldBeAdmin = initialLoginRequest.ShouldBeAdmin,
			Password = "InvalidPass"
		};
		var invalidLoginResult = await userService.LoginUserAsync(loginRequestWithInvalidPasword, id);
		Assert.False(invalidLoginResult.Success);
	}
}