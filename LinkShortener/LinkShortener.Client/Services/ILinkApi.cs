using LinkShortener.Client.Dtos;
using Refit;

namespace LinkShortener.Client.Services;

public interface ILinkApi
{
	[Post("/api/links")]
	Task<LinkDto> CreateLinkAsync(LinkCreateDto dto);

	[Get("/api/links")]
	Task<PagedResult<LinkDto>> GetLinksByUserAsync(int startIndex, int pageSize, bool activeOnly);

	[Patch("/api/links/{linkId}")]
	Task<LinkDto?> UpdateLinkAsync(long linkId, LinkEditDto dto);
}
