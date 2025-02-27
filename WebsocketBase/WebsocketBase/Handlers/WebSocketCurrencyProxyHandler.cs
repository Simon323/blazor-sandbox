using System.Net.WebSockets;
using System.Text;
using WebsocketBase.Shared.Messages;

namespace WebsocketBase.Handlers
{
	public class WebSocketCurrencyProxyHandler
	{
		private static readonly List<WebSocket> _connections = new List<WebSocket>();
		private static Task _proxyTask;
		private static CancellationTokenSource _proxyCts;

		public async Task HandleWebSocketAsync(HttpContext context)
		{
			if (context.WebSockets.IsWebSocketRequest)
			{
				using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
				_connections.Add(webSocket);
				Console.WriteLine($"User connected. Total users: {_connections.Count}");

				if (_proxyTask == null || _proxyTask.IsCompleted)
				{
					_proxyCts = new CancellationTokenSource();
					_proxyTask = ProxyCurrencyRates(_proxyCts.Token);
				}

				var receiveTask = ReceiveMessages(webSocket);

				await receiveTask;

				_connections.Remove(webSocket);
				Console.WriteLine($"User disconnected. Total users: {_connections.Count}");

				if (_connections.Count == 0)
				{
					_proxyCts.Cancel();
					try
					{
						await _proxyTask;
					}
					catch (TaskCanceledException)
					{
						// Task was canceled, no further action needed
					}
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

		async Task ProxyCurrencyRates(CancellationToken token)
		{
			using var clientWebSocket = new ClientWebSocket();
			await clientWebSocket.ConnectAsync(new Uri("ws://localhost:8080"), token);

			var buffer = new byte[1024];
			while (clientWebSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
			{
				try
				{
					var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
					if (result.MessageType == WebSocketMessageType.Text)
					{
						foreach (var socket in _connections)
						{
							if (socket.State == WebSocketState.Open)
							{
								await socket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), WebSocketMessageType.Text, true, token);
							}
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
	}
}
