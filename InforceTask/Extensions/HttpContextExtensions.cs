namespace InforceTask.Extensions;

public static class HttpContextExtensions
{
	public static Guid? GetUserId(this HttpContext httpContext)
	{
		if (httpContext.User?.Claims?.Any() != true)
		{
			return null;
		}

		return Guid.Parse(httpContext.User.Claims.First(x => x.Type == Constants.ClaimTypes.Id).Value);
	}
}