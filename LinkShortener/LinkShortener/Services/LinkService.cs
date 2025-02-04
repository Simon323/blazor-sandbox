using LinkShortener.Client.Dtos;
using LinkShortener.Client.Interfaces;
using LinkShortener.Data;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Services;

public class LinkService : ILinkService
{
	private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
	private readonly IShortCodeGeneratorService _shortCodeGeneratorService;
	private readonly IConfiguration _configuration;

	public LinkService(IDbContextFactory<ApplicationDbContext> contextFactory, IShortCodeGeneratorService shortCodeGeneratorService, IConfiguration configuration)
	{
		_contextFactory = contextFactory;
		_shortCodeGeneratorService = shortCodeGeneratorService;
		_configuration = configuration;
	}

	public async Task<LinkDto> CreateLinkAsync(LinkCreateDto dto)
	{
		var domain = _configuration["Domain"] ?? throw new InvalidOperationException($"Domain is not defined in configuration");
		var shortCode = await _shortCodeGeneratorService.GenerateShortCodeAsync();

		var link = new Link
		{
			LongUrl = dto.LongUrl,
			ShortCode = shortCode,
			ShortUrl = $"{domain.TrimEnd('/')}/{shortCode}",
			UserId = dto.UserId,
			IsActive = true
		};

		await using var context = _contextFactory.CreateDbContext();
		context.Links.Add(link);
		await context.SaveChangesAsync();

		return new LinkDto
		{
			Id = link.Id,
			LongUrl = dto.LongUrl,
			ShortUrl = link.ShortCode,
			IsActive = link.IsActive
		};
	}
}
