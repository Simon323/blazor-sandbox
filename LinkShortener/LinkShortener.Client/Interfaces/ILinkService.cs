using LinkShortener.Client.Dtos;

namespace LinkShortener.Client.Interfaces;

public interface ILinkService
{
	Task<LinkDto> CreateLinkAsync(LinkCreateDto dto);

	Task<PagedResult<LinkDto>> GetLinksByUserAsync(string userId, int startIndex, int pageSize, bool aciveOnly);

	Task<LinkDto?> UpdateLinkAsync(LinkEditDto dto);
}
