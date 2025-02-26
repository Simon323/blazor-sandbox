using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebsocketBase.Shared.Messages;

namespace WebsocketBase.Handlers;

public class WebSocketHandler
{
	private static readonly ConcurrentDictionary<WebSocket, bool> _clients = new();
	private static readonly Random _random = new();

	public async Task HandleWebSocketAsync(HttpContext context)
	{
		if (context.WebSockets.IsWebSocketRequest)
		{
			using WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
			_clients.TryAdd(webSocket, true);

			try
			{
				await SendCurrencyRatesAsync(webSocket);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"WebSocket error: {ex.Message}");
			}
			finally
			{
				_clients.TryRemove(webSocket, out _);
				await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
				webSocket.Dispose();
			}
		}
		else
		{
			context.Response.StatusCode = 400;
		}
	}

	private async Task SendCurrencyRatesAsync(WebSocket webSocket)
	{
		while (webSocket.State == WebSocketState.Open)
		{
			var rates = GenerateRandomRates();
			var json = JsonSerializer.Serialize(rates);

			var buffer = Encoding.UTF8.GetBytes(json);
			await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

			await Task.Delay(5000); // Update every 5 seconds
		}
	}

	private List<CurrencyRate> GenerateRandomRates()
	{
		return new List<CurrencyRate>
		{
			new() { Name = "USD", BuyPrice = (_random.Next(370, 390) / 100.0).ToString("0.00"), SellPrice = (_random.Next(380, 400) / 100.0).ToString("0.00") },
			new() { Name = "EUR", BuyPrice = (_random.Next(440, 460) / 100.0).ToString("0.00"), SellPrice = (_random.Next(450, 470) / 100.0).ToString("0.00") }
		};
	}
}
