using System.Text;
using InforceTask.Options;
using Microsoft.IdentityModel.Tokens;

namespace InforceTask.Domain.Services;

public static class OptionsExtensions
{
	public static SymmetricSecurityKey GetSecurityKey(this JwtOptions options)
	{
		return new(Encoding.ASCII.GetBytes(options.Secret));
	}
}