using System.Text.Json.Serialization;

namespace WebsocketBase.Shared.Messages;

public class CurrencyRate
{
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("buyPrice")]
	public string BuyPrice { get; set; }

	[JsonPropertyName("sellPrice")]
	public string SellPrice { get; set; }
}
