using System.IdentityModel.Tokens.Jwt;

namespace InforceTask;

public static class Constants
{
	public static class Routes
	{
		public const string ApiControllerName = "api/[controller]";

		public static class About
		{
			public const string Edit = "edit";

			public const string Current = "current";
		}

		public static class Users
		{
			public const string Login = "login";

			public const string Refresh = "refresh";
		}

		public static class Urls
		{
			public const string All = "all";

			public const string Url = "url";

			public const string UrlWithIdentifier = $"{Url}/{{id}}";
		}
	}

}