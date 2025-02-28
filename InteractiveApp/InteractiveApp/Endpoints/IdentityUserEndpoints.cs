using InteractiveApp.Client.Extensions;
using InteractiveApp.Client.Interfaces;
using InteractiveApp.Services;
using System.Security.Claims;

namespace InteractiveApp.Endpoints;

public static class IdentityUserEndpoints
{
	public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
	{
		var linksGroup = app.MapGroup("/api/users").RequireAuthorization();
		linksGroup.MapPost("/add-random-roles", async (IIdentityUserService identityUserService, ClaimsPrincipal principal) =>
		{
			var userId = principal.GetUserId();
			await identityUserService.AddRandomRoles(userId);
			return Results.Ok();
		});

		linksGroup.MapDelete("/delete-all-roles", async (IIdentityUserService identityUserService, ClaimsPrincipal principal) =>
		{
			var userId = principal.GetUserId();
			await identityUserService.DeleteAllRoles(userId);
			return Results.Ok();
		});

		linksGroup.MapGet("/roles", async (IUserRequirementsService userRequirementsService, ClaimsPrincipal principal) =>
		{
			var userId = principal.GetUserId();
			var result = await userRequirementsService.GetUserRoles(userId);
			return Results.Ok(result);
		});

		return linksGroup;
	}
}
