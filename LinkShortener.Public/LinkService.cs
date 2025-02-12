using LinkShortener.Data;

namespace LinkShortener.Public;

public interface ILinkService
{
	Task<string?> GetLongUrlByShortCodeAsync(string shortCode);
}

public class LinkService : ILinkService
{
	private readonly ApplicationDbContext _dbContext;

	public LinkService(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<string?> GetLongUrlByShortCodeAsync(string shortCode)
	{
		var link = _dbContext.Links.FirstOrDefault(l => l.ShortCode == shortCode);
		if (link == null)
			return null;

		var linkAnalytic = new LinkAnalytic
		{
			LinkId = link.Id,
			CreatedAt = DateTime.Now
		};

		_dbContext.LinkAnalytics.Add(linkAnalytic);
		await _dbContext.SaveChangesAsync();
		return link.LongUrl;
	}
}
