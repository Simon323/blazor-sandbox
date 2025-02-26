using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebsocketBase.Shared.Messages;

namespace WebsocketBase.Handlers;

public class WebSocketHandler
{
	private static readonly ConcurrentDictionary<WebSocket, CancellationTokenSource> _clients = new();
	private static readonly Random _random = new();

	public async Task HandleWebSocketAsync(HttpContext context)
	{
		if (context.WebSockets.IsWebSocketRequest)
		{
			var webSocket = await context.WebSockets.AcceptWebSocketAsync();
			var cts = new CancellationTokenSource();
			_clients.TryAdd(webSocket, cts);

			Console.WriteLine("🟢 Nowe połączenie WebSocket!");

			try
			{
				await SendCurrencyRatesAsync(webSocket, cts.Token);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ WebSocket error: {ex.Message}");
			}
			finally
			{
				await DisconnectClient(webSocket);
			}
		}
		else
		{
			context.Response.StatusCode = 400;
		}
	}

	private async Task SendCurrencyRatesAsync(WebSocket webSocket, CancellationToken token)
	{
		while (webSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
		{
			var rates = GenerateRandomRates();
			var json = JsonSerializer.Serialize(rates);
			var buffer = Encoding.UTF8.GetBytes(json);

			try
			{
				await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, token);
			}
			catch (WebSocketException)
			{
				Console.WriteLine("⚠ WebSocket zamknięty - przerywam wysyłanie.");
				break;
			}

			try
			{
				// Jeśli klient się rozłączył, pętla zostanie natychmiast przerwana
				await Task.Delay(5000, token);
			}
			catch (TaskCanceledException)
			{
				break; // Natychmiast przerywamy pętlę
			}
		}
	}

	public async Task DisconnectClient(WebSocket webSocket)
	{
		if (_clients.TryRemove(webSocket, out var cts))
		{
			cts.Cancel(); // Anuluj `Task.Delay`
			cts.Dispose();

			if (webSocket.State == WebSocketState.Open)
			{
				await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected", CancellationToken.None);
			}

			Console.WriteLine("🔴 Klient WebSocket rozłączony.");
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