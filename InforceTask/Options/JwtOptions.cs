using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace InforceTask.Options;

public sealed class JwtOptions
{
	private SymmetricSecurityKey? _key;

	public string Secret { get; init; }

	public TimeSpan AccessTokenLifetime { get; init; }

	public TimeSpan RefreshTokenLifetime { get; init; }

	public SymmetricSecurityKey Key => this._key ??= new(Encoding.ASCII.GetBytes(this.Secret));
}