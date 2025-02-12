using LinkShortener.Client.Dtos;
using LinkShortener.Client.Interfaces;

namespace LinkShortener.Client.Services;

public class LinkApiProxy : ILinkService
{
	private readonly ILinkApi _linkApi;

	public LinkApiProxy(ILinkApi linkApi)
	{
		_linkApi = linkApi;
	}

	public Task<LinkDto> CreateLinkAsync(LinkCreateDto dto) =>
		_linkApi.CreateLinkAsync(dto);

	public Task<PagedResult<LinkDto>> GetLinksByUserAsync(string userId, int startIndex, int pageSize, bool aciveOnly) =>
		_linkApi.GetLinksByUserAsync(startIndex, pageSize, aciveOnly);

	public async Task<LinkDto?> UpdateLinkAsync(LinkEditDto dto) =>
		await _linkApi.UpdateLinkAsync(dto.Id, dto);

	public Task DeleteLinkAsync(long id, string userId) =>
		_linkApi.DeleteLinkAsync(id);
}
