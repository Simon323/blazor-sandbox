using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebsocketBase.Shared.Messages;

namespace WebsocketBase.Handlers
{
	public class WebSocketCurrencyHandler
	{
		public async Task HandleWebSocketAsync(HttpContext context)
		{
			if (context.WebSockets.IsWebSocketRequest)
			{
				using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
				var cts = new CancellationTokenSource();

				// Uruchamiamy równolegle wysy³anie i odbieranie
				var sendTask = SendCurrencyRates(webSocket, cts.Token);
				var receiveTask = ReceiveMessages(webSocket, cts.Token);

				// Czekamy a¿ jedno z zadañ siê zakoñczy (np. klient wysy³a close frame)
				await Task.WhenAny(sendTask, receiveTask);

				// Anulujemy drugie zadanie
				cts.Cancel();

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
				var currencyRate = new CurrencyRate
				{
					Name = "USD/EUR",
					BuyPrice = "0.92",
					SellPrice = "0.93"
				};

				var json = JsonSerializer.Serialize(currencyRate);
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
	}
}
