using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
	const string GetEndpointName = "GetGame";

	public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
	{
		// WithParameterValidation() come from MinimalApis.Extensions package
		var group = app.MapGroup("/games").WithParameterValidation();

		// GET /games
		group.MapGet("/", (GameStoreContext context) =>
			context.Games
				.Include(game => game.Genre)
				.Select(x => x.ToGameSummaryDto())
				.AsNoTracking());

		// GET /games/{id}
		group.MapGet("/{id}", (int id, GameStoreContext context) =>
		{
			var game = context.Games.Find(id);
			return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
		}).WithName(GetEndpointName);

		// POST /games
		group.MapPost("/", (CreateGameDto newGame, GameStoreContext context) =>
		{
			var game = newGame.ToEntity();

			context.Games.Add(game);
			context.SaveChanges();

			return Results.CreatedAtRoute(GetEndpointName, new { id = game.Id }, game.ToGameSummaryDto());
		});

		// PUT /games/{id}
		group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, GameStoreContext context) =>
		{
			var existingGame = context.Games.Find(id);

			if (existingGame is null)
			{
				return Results.NotFound();
			}

			context.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));
			context.SaveChanges();

			return Results.NoContent();
		});

		// DELETE /games/{id}
		group.MapDelete("/{id}", (int id, GameStoreContext context) =>
		{
			context.Games
				.Where(game => game.Id == id)
				.ExecuteDelete();
			return Results.NoContent();
		});

		return group;
	}
}
