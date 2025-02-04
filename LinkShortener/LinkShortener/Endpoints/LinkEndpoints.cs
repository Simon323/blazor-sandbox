using LinkShortener.Client.Dtos;
using LinkShortener.Client.Extensions;
using LinkShortener.Client.Interfaces;
using System.Security.Claims;

namespace LinkShortener.Endpoints;

public static class LinkEndpoints
{
	public static IEndpointRouteBuilder MapLinkEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapPost("/api/links", async (LinkCreateDto dto, ILinkService linkservice, ClaimsPrincipal principal) =>
		{
			var userId = principal.GetUserId();
			if (userId != dto.UserId)
				return Results.Unauthorized();

			var link = await linkservice.CreateLinkAsync(dto);
			return Results.Ok(link);

		}).RequireAuthorization();

		return app;
	}
}
