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
			ShortUrl = link.ShortUrl,
			IsActive = link.IsActive
		};
	}

	public async Task DeleteLinkAsync(long id, string userId)
	{
		await using var context = _contextFactory.CreateDbContext();
		var link = await context.Links
			.Include(l => l.Analytics)
			.FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
		if (link is null)
			return;
		if (link.Analytics.Count > 0)
			context.LinkAnalytics.RemoveRange(link.Analytics);
		context.Links.Remove(link);
		await context.SaveChangesAsync();
	}

	public async Task<DashboardDataDto> GetDashboardDataAsync(string userId)
	{
		var totalLinksTask = GetTotalLinks(userId);
		var totalClicksTask = GetTotalClicks(userId);
		var totalActiveLinksTask = GetTotalActiveLinks(userId);
		var totalLinksTodayTask = GetTotalLinksToday(userId);
		var totalClicksTodayTask = GetTotalClicksToday(userId);

		await Task.WhenAll(totalLinksTask, totalClicksTask, totalActiveLinksTask, totalLinksTodayTask, totalClicksTodayTask);

		var totalLinks = await totalLinksTask;
		var totalClicks = await totalClicksTask;
		var totalActiveLinks = await totalActiveLinksTask;
		var totalLinksToday = await totalLinksTodayTask;
		var totalClicksToday = await totalClicksTodayTask;
		var totalInactiveLinks = totalLinks - totalActiveLinks;

		return new DashboardDataDto(totalLinks, totalClicks, totalActiveLinks, totalInactiveLinks, totalLinksToday, totalClicksToday);
	}

	public async Task<LinkDetailsDto?> GetLinkAsync(long id, string userId)
	{
		await using var context = _contextFactory.CreateDbContext();
		var link = await context.Links
			.Include(l => l.Analytics)
			.FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

		if (link is null)
			return null;

		var linkAnalytics = (link.Analytics
			.Select(a => new LinkAnalyticDto
			{
				Id = a.Id,
				ClickedAt = a.CreatedAt,
				LinkId = a.LinkId,
			}).ToArray())
			?? [];

		var linkDto = new LinkDto
		{
			Id = link.Id,
			IsActive = link.IsActive,
			LongUrl = link.LongUrl,
			ShortUrl = link.ShortUrl,
			TotalClicks = linkAnalytics.Length
		};

		return new LinkDetailsDto(linkDto, linkAnalytics);
	}

	public async Task<PagedResult<LinkDto>> GetLinksByUserAsync(string userId, int startIndex, int pageSize, bool aciveOnly)
	{
		await using var context = _contextFactory.CreateDbContext();

		var query = context.Links.Where(l => l.UserId == userId);

		var queryResult = aciveOnly ? query.Where(l => l.IsActive) : query;

		var totalLinks = await queryResult.CountAsync();
		var links = await queryResult
			.OrderByDescending(l => l.Id)
			.Skip(startIndex)
			.Take(pageSize)
			.Select(l => new LinkDto
			{
				Id = l.Id,
				LongUrl = l.LongUrl,
				ShortUrl = l.ShortUrl,
				IsActive = l.IsActive,
				TotalClicks = l.Analytics.Count
			}).ToArrayAsync();

		return new PagedResult<LinkDto>(links, totalLinks);
	}

	public async Task<LinkDto?> UpdateLinkAsync(LinkEditDto dto)
	{
		await using var context = _contextFactory.CreateDbContext();
		var link = await context.Links
			.FirstOrDefaultAsync(l => l.Id == dto.Id && l.UserId == dto.UserId);

		if (link is null)
			return null;

		link.LongUrl = dto.LongUrl;
		link.IsActive = dto.IsActive;

		context.Links.Update(link);
		await context.SaveChangesAsync();

		return new LinkDto
		{
			Id = link.Id,
			LongUrl = link.LongUrl,
			ShortUrl = link.ShortUrl,
			IsActive = link.IsActive,
		};
	}

	private async Task<int> GetTotalLinks(string userId)
	{
		await using var context = _contextFactory.CreateDbContext();
		return await context.Links.CountAsync(l => l.UserId == userId);
	}

	private async Task<int> GetTotalClicks(string userId)
	{
		await using var context = _contextFactory.CreateDbContext();
		return await context.LinkAnalytics.CountAsync(a => a.Link.UserId == userId);
	}

	private async Task<int> GetTotalActiveLinks(string userId)
	{
		await using var context = _contextFactory.CreateDbContext();
		return await context.Links.CountAsync(l => l.UserId == userId && l.IsActive);
	}

	private async Task<int> GetTotalLinksToday(string userId)
	{
		return await Task.FromResult(10);
	}

	private async Task<int> GetTotalClicksToday(string userId)
	{
		await using var context = _contextFactory.CreateDbContext();
		return await context.LinkAnalytics.CountAsync(a => a.Link.UserId == userId && a.CreatedAt.Date == DateTime.Today.Date);
	}
}
