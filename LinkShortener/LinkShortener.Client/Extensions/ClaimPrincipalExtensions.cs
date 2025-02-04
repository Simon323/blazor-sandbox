using System.Security.Claims;

namespace LinkShortener.Client.Extensions;

public static class ClaimPrincipalExtensions
{
	public static string? GetUserId(this ClaimsPrincipal principal) =>
		principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
