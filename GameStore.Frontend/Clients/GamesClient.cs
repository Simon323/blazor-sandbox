using GameStore.Frontend.Exceptions;
using GameStore.Frontend.Models;

namespace GameStore.Frontend.Clients;

public class GamesClient(HttpClient httpClient)
{
	public async Task<GameSummary[]> GetGames()
		=> await httpClient.GetFromJsonAsync<GameSummary[]>("games") ?? [];

	public async Task AddGame(GameDetails game)
		=> await httpClient.PostAsJsonAsync("games", game);

	public async Task<GameDetails> GetGame(int id)
		=> await httpClient.GetFromJsonAsync<GameDetails>($"games/{id}")
		?? throw new NotFoundException("Could not find game");

	public async Task UpdateGame(GameDetails updatedGame)
		=> await httpClient.PutAsJsonAsync($"games/{updatedGame.Id}", updatedGame);

	public async Task DeleteGame(int id)
		=> await httpClient.DeleteAsync($"games/{id}");
}
