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

	public Task<LinkDto> CreateLinkAsync(LinkCreateDto dto) => _linkApi.CreateLinkAsync(dto);
}
