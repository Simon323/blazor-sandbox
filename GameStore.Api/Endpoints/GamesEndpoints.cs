using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
	const string GetEndpointName = "GetGame";

	private static readonly List<GameDto> games = [
		new GameDto(1, "The Last of Us Part II", "Action", 199.99m, new DateOnly(2020, 6, 19)),
		new GameDto(2, "Cyberpunk 2077", "RPG", 199.99m, new DateOnly(2020, 12, 10)),
		new GameDto(3, "Doom Eternal", "Action", 199.99m, new DateOnly(2020, 3, 20)),
		new GameDto(4, "Half-Life: Alyx", "Action", 199.99m, new DateOnly(2020, 3, 23)),
		new GameDto(5, "Final Fantasy VII Remake", "RPG", 199.99m, new DateOnly(2020, 4, 10)),
		new GameDto(6, "Ghost of Tsushima", "Action", 199.99m, new DateOnly(2020, 7, 17)),
		new GameDto(7, "Hades", "RPG", 199.99m, new DateOnly(2020, 9, 17)),
		new GameDto(8, "Demon's Souls", "RPG", 199.99m, new DateOnly(2020, 11, 12)),
		new GameDto(9, "Assassin's Creed Valhalla", "Action", 199.99m, new DateOnly(2020, 11, 10)),
	];

	public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
	{
		// WithParameterValidation() come from MinimalApis.Extensions package
		var group = app.MapGroup("/games").WithParameterValidation();

		// GET /games
		group.MapGet("/", () => games);

		// GET /games/{id}
		group.MapGet("/{id}", (int id) =>
		{
			var game = games.SingleOrDefault(x => x.Id == id);
			return game is null ? Results.NotFound() : Results.Ok(game);
		}).WithName(GetEndpointName);

		// POST /games
		group.MapPost("/", (CreateGameDto newGame, GameStoreContext context) =>
		{
			var game = new Game
			{
				Name = newGame.Name,
				Genre = context.Genres.Find(newGame.GenreId),
				GenreId = newGame.GenreId,
				Price = newGame.Price,
				ReleaseDate = newGame.ReleaseDate
			};

			context.Games.Add(game);
			context.SaveChanges();

			var result = new GameDto(game.Id, game.Name, game.Genre!.Name, game.Price, game.ReleaseDate);

			return Results.CreatedAtRoute(GetEndpointName, new { id = game.Id }, result);
		});

		// PUT /games/{id}
		group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
		{
			var index = games.FindIndex(x => x.Id == id);

			if (index == -1)
			{
				return Results.NotFound();
			}

			games[index] = games[index] with
			{
				Name = updatedGame.Name ?? games[index].Name,
				Genre = updatedGame.Genre ?? games[index].Genre,
				Price = updatedGame.Price,
				ReleaseDate = updatedGame.ReleaseDate,
			};

			return Results.NoContent();
		});

		// DELETE /games/{id}
		group.MapDelete("/{id}", (int id) =>
		{
			games.RemoveAll(game => game.Id == id);
			return Results.NoContent();
		});

		return group;
	}
}
