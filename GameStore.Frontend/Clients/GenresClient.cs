using GameStore.Frontend.Models;

namespace GameStore.Frontend.Clients;

public class GenresClient
{
	private readonly Genre[] _genres =
	[
		new() { Id = 1, Name = "Action" },
		new() { Id = 2, Name = "Adventure" },
		new() { Id = 3, Name = "RPG" },
		new() { Id = 4, Name = "Sport" },
	];

	public Genre[] GetGenres() => [.. _genres];
}
