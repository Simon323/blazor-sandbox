using GameStore.Api.Data;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{
	public static RouteGroupBuilder MapGenreEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/genres");

		group.MapGet("/", async (GameStoreContext db) =>
		{
			var genres = await db.Genres
				.Select(x => x.ToDto())
				.AsNoTracking()
				.ToListAsync();

			return Results.Ok(genres);
		});

		return group;
	}
}
