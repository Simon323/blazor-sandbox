using System.Security.Claims;

namespace InteractiveApp.Client.Extensions;

public static class ClaimsPrincipalExtensions
{
	public static List<string> GetUserRoles(this ClaimsPrincipal claimsPrincipal)
	{
		if (claimsPrincipal.Identity?.IsAuthenticated == true)
		{
			return claimsPrincipal.FindAll(ClaimTypes.Role).Select(c => c.Value).Order().ToList();
		}
		return [];
	}

	public static string? GetUserId(this ClaimsPrincipal principal) =>
		principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
