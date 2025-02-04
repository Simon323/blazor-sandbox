using LinkShortener.Data;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Services;

public interface IShortCodeGeneratorService
{
	Task<string> GenerateShortCodeAsync();
}

public class ShortCodeGeneratorService : IShortCodeGeneratorService
{
	private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

	public ShortCodeGeneratorService(IDbContextFactory<ApplicationDbContext> contextFactory)
	{
		this._contextFactory = contextFactory;
	}

	public async Task<string> GenerateShortCodeAsync()
	{
		var shortCode = GenerateShortCode(6);

		await using var context = this._contextFactory.CreateDbContext();

		while (await context.Links.AnyAsync(l => l.ShortCode == shortCode))
		{
			shortCode = GenerateShortCode(6);
		}

		return shortCode;
	}

	private static string GenerateShortCode(int length)
	{
		const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		int availableCharacters = characters.Length;
		var shortCoders = Enumerable.Repeat(characters, length)
			.Select(i =>
			{
				var randomNumber = Random.Shared.Next(availableCharacters);
				return i[randomNumber];
			}).ToArray();

		return new string(shortCoders);
	}
}
