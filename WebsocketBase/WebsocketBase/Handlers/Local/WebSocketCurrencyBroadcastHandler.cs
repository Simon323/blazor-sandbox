using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebsocketBase.Shared.Messages;

namespace WebsocketBase.Handlers.Local
{
	public class WebSocketCurrencyBroadcastHandler
	{
		private static readonly List<WebSocket> _connections = new List<WebSocket>();
		private static readonly Random _random = new Random();
		private static Task _broadcastTask;
		private static CancellationTokenSource _broadcastCts;

		public async Task HandleWebSocketAsync(HttpContext context)
		{
			if (context.WebSockets.IsWebSocketRequest)
			{
				using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
				_connections.Add(webSocket);
				Console.WriteLine($"User connected. Total users: {_connections.Count}");

				if (_broadcastTask == null || _broadcastTask.IsCompleted)
				{
					_broadcastCts = new CancellationTokenSource();
					_broadcastTask = BroadcastCurrencyRates(_broadcastCts.Token);
				}

				var receiveTask = ReceiveMessages(webSocket);

				await receiveTask;

				_connections.Remove(webSocket);
				Console.WriteLine($"User disconnected. Total users: {_connections.Count}");

				if (_connections.Count == 0)
				{
					_broadcastCts.Cancel();
					await _broadcastTask;
				}

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

		async Task BroadcastCurrencyRates(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				var currencyRates = GenerateRandomRates();
				var json = JsonSerializer.Serialize(currencyRates);
				var buffer = Encoding.UTF8.GetBytes(json);

				foreach (var socket in _connections)
				{
					if (socket.State == WebSocketState.Open)
					{
						await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, token);
					}
				}

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

		async Task ReceiveMessages(WebSocket socket)
		{
			var buffer = new byte[1024];
			while (socket.State == WebSocketState.Open)
			{
				var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
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
}
