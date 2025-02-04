using LinkShortener.Client.Dtos;

namespace LinkShortener.Client.Interfaces;

public interface ILinkService
{
	Task<LinkDto> CreateLinkAsync(LinkCreateDto dto);
}
