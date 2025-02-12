namespace LinkShortener.Client.Dtos;

public record LinkDetailsDto(LinkDto Link, LinkAnalyticDto[] LinkAnalytic);
