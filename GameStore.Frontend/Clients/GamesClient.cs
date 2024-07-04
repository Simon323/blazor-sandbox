using GameStore.Frontend.Models;

namespace GameStore.Frontend.Clients;

public class GamesClient
{
	private readonly List<GameSummary> _games =
	[
		new() { Id = 1, Name = "Game 1", Genre = "Action", Price = 10.0M, ReleaseDate = new DateOnly(2021, 1, 1) },
		new() { Id = 2, Name = "Game 2", Genre = "Sport", Price = 21.0M, ReleaseDate = new DateOnly(1992, 1, 1) },
		new() { Id = 3, Name = "Game 3", Genre = "RPG", Price = 15.0M, ReleaseDate = new DateOnly(2000, 1, 1) },
		new() { Id = 4, Name = "Game 4", Genre = "Action", Price = 30.0M, ReleaseDate = new DateOnly(2010, 1, 1) },
	];

	public GameSummary[] GetGames() => [.. _games];
}
