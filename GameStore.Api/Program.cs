using GameStore.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

const string GetEndpointName = "GetGame";

List<GameDto> games = [
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

// GET /games
app.MapGet("/games", () => games);

// GET /games/{id}
app.MapGet("/games/{id}", (int id) =>
{
	var game = games.SingleOrDefault(x => x.Id == id);
	if (game is null)
	{
		return Results.NotFound();
	}

	return Results.Ok(game);
}).WithName(GetEndpointName);

// POST /games
app.MapPost("/games", (CreateGameDto newGame) =>
{
	var game = new GameDto(games.Max(x => x.Id) + 1, newGame.Name, newGame.Genre, newGame.Price, newGame.ReleaseDate);
	games.Add(game);

	return Results.CreatedAtRoute(GetEndpointName, new { id = game.Id }, game);
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Game Store API");
	c.EnableTryItOutByDefault();
});

app.Run();