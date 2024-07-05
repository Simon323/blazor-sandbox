using GameStore.Frontend.Models;

namespace GameStore.Frontend.Clients;

public class GamesClient
{
	private readonly Genre[] _genres;

	private readonly List<GameSummary> _games =
	[
		new() { Id = 1, Name = "Game 1", Genre = "Action", Price = 10.0M, ReleaseDate = new DateOnly(2021, 1, 1) },
		new() { Id = 2, Name = "Game 2", Genre = "Sport", Price = 21.0M, ReleaseDate = new DateOnly(1992, 1, 1) },
		new() { Id = 3, Name = "Game 3", Genre = "RPG", Price = 15.0M, ReleaseDate = new DateOnly(2000, 1, 1) },
		new() { Id = 4, Name = "Game 4", Genre = "Action", Price = 30.0M, ReleaseDate = new DateOnly(2010, 1, 1) },
	];

	public GameSummary[] GetGames() => [.. _games];

	public GamesClient(GenresClient genresClient)
	{
		_genres = genresClient.GetGenres();
	}

	public void AddGame(GameDetails game)
	{
		Genre genre = GetGenreById(game.GenreId);

		var gameSummary = new GameSummary
		{
			Id = _games.Count + 1,
			Name = game.Name,
			Genre = genre.Name,
			Price = game.Price,
			ReleaseDate = game.ReleaseDate,
		};

		_games.Add(gameSummary);
	}

	public GameDetails GetGame(int id)
	{
		var game = GetGameSummaryById(id);
		var genre = _genres.Single(x => string.Equals(x.Name, game.Genre, StringComparison.OrdinalIgnoreCase));

		return new GameDetails
		{
			Id = game.Id,
			Name = game.Name,
			GenreId = genre.Id.ToString(),
			Price = game.Price,
			ReleaseDate = game.ReleaseDate,
		};
	}

	public void UpdateGame(GameDetails updatedGame)
	{
		Genre genre = GetGenreById(updatedGame.GenreId);
		var existingGame = GetGameSummaryById(updatedGame.Id);

		existingGame.Name = updatedGame.Name;
		existingGame.Genre = genre.Name;
		existingGame.Price = updatedGame.Price;
		existingGame.ReleaseDate = updatedGame.ReleaseDate;
	}

	public void DeleteGame(int id)
	{
		var game = GetGameSummaryById(id);
		_games.Remove(game);
	}

	private GameSummary GetGameSummaryById(int id)
	{
		var game = _games.Find(x => x.Id == id);
		ArgumentNullException.ThrowIfNull(game);
		return game;
	}

	private Genre GetGenreById(string? id)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(id);
		return _genres.Single(x => x.Id == int.Parse(id));
	}
}
