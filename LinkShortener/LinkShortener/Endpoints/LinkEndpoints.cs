using LinkShortener.Client.Dtos;
using LinkShortener.Client.Extensions;
using LinkShortener.Client.Interfaces;
using System.Security.Claims;

namespace LinkShortener.Endpoints;

public static class LinkEndpoints
{
	public static IEndpointRouteBuilder MapLinkEndpoints(this IEndpointRouteBuilder app)
	{
		var linksGroup = app.MapGroup("/api/links").RequireAuthorization();

		linksGroup.MapPost("", async (LinkCreateDto dto, ILinkService linkservice, ClaimsPrincipal principal) =>
		{
			var userId = principal.GetUserId();
			if (userId != dto.UserId)
				return Results.Unauthorized();

			var link = await linkservice.CreateLinkAsync(dto);
			return Results.Ok(link);

		});

		linksGroup.MapGet("", async (ILinkService linkservice, ClaimsPrincipal principal, int startIndex, int pageSize, bool activeOnly) =>
		{
			var userId = principal.GetUserId();
			var links = await linkservice.GetLinksByUserAsync(userId, startIndex, pageSize, activeOnly);
			return Results.Ok(links);
		});

		linksGroup.MapPatch("/{linkId:long}", async (long linkId, LinkEditDto dto, ILinkService linkService, ClaimsPrincipal principal) =>
		{
			var userId = principal.GetUserId();
			if (userId != dto.UserId)
				return Results.Unauthorized();
			if (linkId != dto.Id)
				return Results.NotFound();


			var link = await linkService.UpdateLinkAsync(dto);
			if (link is null)
				return Results.NotFound(linkId);

			return Results.Ok(link);
		});

		linksGroup.MapDelete("/{linkId:long}", async (long linkId, ILinkService linkService, ClaimsPrincipal principal) =>
		{
			var userId = principal.GetUserId();
			await linkService.DeleteLinkAsync(linkId, userId);
			return Results.NoContent();
		});

		linksGroup.MapGet("/{linkId:long}", async (long linkId, ILinkService linkService, ClaimsPrincipal principal) =>
		{
			var userId = principal.GetUserId();
			var linkDetailsDto = await linkService.GetLinkAsync(linkId, userId);
			if (linkDetailsDto is null)
				return Results.NotFound(linkId);
			return Results.Ok(linkDetailsDto);
		});

		linksGroup.MapGet("/dashboard", async (ILinkService linkService, ClaimsPrincipal principal) =>
		{
			var userId = principal.GetUserId();
			var dashboardData = await linkService.GetDashboardDataAsync(userId!);
			return Results.Ok(dashboardData);
		});

		return linksGroup;
	}
}
