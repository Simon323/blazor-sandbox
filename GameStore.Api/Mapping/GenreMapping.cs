using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GenreMapping
{
	public static Genre ToEntity(this GenreDto dto) => new Genre
	{
		Id = dto.Id,
		Name = dto.Name,
	};

	public static GenreDto ToDto(this Genre entity) => new GenreDto(
		entity.Id,
		entity.Name);
}
