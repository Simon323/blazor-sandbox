using GameStore.Frontend.Models;

namespace GameStore.Frontend.Clients;

public class GenresClient
{
	private readonly Genre[] _genres =
	[
		new() { Id = 1, Name = "Action" },
		new() { Id = 2, Name = "Adventure" },
		new() { Id = 3, Name = "RPG" },
	];

	public Genre[] GetGenres() => [.. _genres];
}
