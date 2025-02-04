using LinkShortener.Client.Dtos;
using Refit;

namespace LinkShortener.Client.Services;

public interface ILinkApi
{
	[Post("/api/links")]
	Task<LinkDto> CreateLinkAsync(LinkCreateDto dto);
}
