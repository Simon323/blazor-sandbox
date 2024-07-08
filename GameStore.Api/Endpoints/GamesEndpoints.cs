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
		group.MapGet("/", async (GameStoreContext context) =>
		{
			//await Task.Delay(10000); // Simulate a slow API
			return await context.Games
				.Include(game => game.Genre)
				.Select(x => x.ToGameSummaryDto())
				.AsNoTracking()
				.ToListAsync();
		});

		// GET /games/{id}
		group.MapGet("/{id}", async (int id, GameStoreContext context) =>
		{
			var game = await context.Games.FindAsync(id);
			return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
		}).WithName(GetEndpointName);

		// POST /games
		group.MapPost("/", async (CreateGameDto newGame, GameStoreContext context) =>
		{
			var game = newGame.ToEntity();

			context.Games.Add(game);
			await context.SaveChangesAsync();

			return Results.CreatedAtRoute(GetEndpointName, new { id = game.Id }, game.ToGameDetailsDto());
		});

		// PUT /games/{id}
		group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext context) =>
		{
			var existingGame = await context.Games.FindAsync(id);

			if (existingGame is null)
			{
				return Results.NotFound();
			}

			context.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));
			await context.SaveChangesAsync();

			return Results.NoContent();
		});

		// DELETE /games/{id}
		group.MapDelete("/{id}", async (int id, GameStoreContext context) =>
		{
			await context.Games
				.Where(game => game.Id == id)
				.ExecuteDeleteAsync();
			return Results.NoContent();
		});

		return group;
	}
}
