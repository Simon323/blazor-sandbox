using System.Net.WebSockets;

namespace WebsocketBase.Handlers.Proxy
{
	public class WebSocketCurrencyIndividualProxyHandler
	{
		private static readonly List<WebSocket> _connections = new List<WebSocket>();

		public async Task HandleWebSocketAsync(HttpContext context)
		{
			if (context.WebSockets.IsWebSocketRequest)
			{
				using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
				_connections.Add(webSocket);
				Console.WriteLine($"User connected. Total users: {_connections.Count}");

				var cts = new CancellationTokenSource();
				var proxyTask = ProxyCurrencyRates(webSocket, cts.Token);
				var receiveTask = ReceiveMessages(webSocket, cts);

				await Task.WhenAny(proxyTask, receiveTask);

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

		async Task ProxyCurrencyRates(WebSocket clientWebSocket, CancellationToken token)
		{
			using var serverWebSocket = new ClientWebSocket();
			await serverWebSocket.ConnectAsync(new Uri("ws://localhost:8080"), token);

			var buffer = new byte[1024];
			while (serverWebSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
			{
				try
				{
					var result = await serverWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
					if (result.MessageType == WebSocketMessageType.Text)
					{
						if (clientWebSocket.State == WebSocketState.Open)
						{
							await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), WebSocketMessageType.Text, true, token);
						}
					}
					else if (result.MessageType == WebSocketMessageType.Close)
					{
						break;
					}
				}
				catch (OperationCanceledException)
				{
					// Operation was canceled, exit the loop
					break;
				}
				catch (WebSocketException ex)
				{
					Console.WriteLine($"WebSocket error: {ex.Message}");
					break;
				}
			}
		}

		async Task ReceiveMessages(WebSocket socket, CancellationTokenSource cts)
		{
			var buffer = new byte[1024];
			while (socket.State == WebSocketState.Open)
			{
				var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				if (result.MessageType == WebSocketMessageType.Close)
				{
					cts.Cancel();
					break;
				}
			}
		}
	}
}
