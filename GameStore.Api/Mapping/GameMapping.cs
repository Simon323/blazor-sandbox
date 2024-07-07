using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GameMapping
{
	public static Game ToEntity(this CreateGameDto dto) => new Game
	{
		Name = dto.Name,
		Price = dto.Price,
		GenreId = dto.GenreId,
		ReleaseDate = dto.ReleaseDate,
	};

	public static Game ToEntity(this UpdateGameDto dto, int id) => new Game
	{
		Id = id,
		Name = dto.Name,
		Price = dto.Price,
		GenreId = dto.GenreId,
		ReleaseDate = dto.ReleaseDate,
	};

	public static GameSummaryDto ToGameSummaryDto(this Game entity) => new GameSummaryDto(
		entity.Id,
		entity.Name,
		entity.Genre!.Name,
		entity.Price,
		entity.ReleaseDate);

	public static GameDetailsDto ToGameDetailsDto(this Game entity) => new GameDetailsDto(
		entity.Id,
		entity.Name,
		entity.GenreId,
		entity.Price,
		entity.ReleaseDate);
}
