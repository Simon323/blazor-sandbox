namespace LinkShortener.Client.Dtos;

public record DashboardDataDto(
	int TotalLinks, int TotalClicks,
	int TotalActiveLinks, int TotalInactiveLinks,
	int TotalLinksCreatedToday, int TotalClicksToday);