using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebsocketBase.Shared.Messages;

namespace WebsocketBase.Handlers.Local;

public class WebSocketCurrencyIndividualHandler
{
	private static readonly List<WebSocket> _connections = new List<WebSocket>();
	private static readonly Random _random = new Random();

	public async Task HandleWebSocketAsync(HttpContext context)
	{
		if (context.WebSockets.IsWebSocketRequest)
		{
			using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
			_connections.Add(webSocket);
			Console.WriteLine($"User connected. Total users: {_connections.Count}");

			var cts = new CancellationTokenSource();

			// Uruchamiamy równolegle wysyłanie i odbieranie
			var sendTask = SendCurrencyRates(webSocket, cts.Token);
			var receiveTask = ReceiveMessages(webSocket, cts.Token);

			// Czekamy aż jedno z zadań się zakończy (np. klient wysyła close frame)
			await Task.WhenAny(sendTask, receiveTask);

			// Anulujemy drugie zadanie
			cts.Cancel();

			_connections.Remove(webSocket);
			Console.WriteLine($"User disconnected. Total users: {_connections.Count}");

			if (webSocket.State != WebSocketState.Closed)
			{
				await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
			}
		}
		else
		{
			context.Response.StatusCode = 400;
		}
	}

	async Task SendCurrencyRates(WebSocket socket, CancellationToken token)
	{
		while (socket.State == WebSocketState.Open && !token.IsCancellationRequested)
		{
			var currencyRates = GenerateRandomRates();
			var json = JsonSerializer.Serialize(currencyRates);
			var buffer = Encoding.UTF8.GetBytes(json);
			await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, token);

			try
			{
				await Task.Delay(TimeSpan.FromSeconds(5), token);
			}
			catch (TaskCanceledException)
			{
				break;
			}
		}
	}

	async Task ReceiveMessages(WebSocket socket, CancellationToken token)
	{
		var buffer = new byte[1024];
		while (socket.State == WebSocketState.Open && !token.IsCancellationRequested)
		{
			var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
			if (result.MessageType == WebSocketMessageType.Close)
			{
				break;
			}
		}
	}

	private List<CurrencyRate> GenerateRandomRates()
	{
		var currencies = new List<string> { "USD", "EUR", "GBP", "JPY", "AUD", "CAD", "CHF", "CNY", "SEK", "NZD" };
		var rates = new List<CurrencyRate>();

		foreach (var currency in currencies)
		{
			rates.Add(new CurrencyRate
			{
				Name = currency,
				BuyPrice = (_random.Next(100, 200) / 100.0).ToString("0.00"),
				SellPrice = (_random.Next(100, 200) / 100.0).ToString("0.00")
			});
		}

		return rates;
	}
}
