using System.Net.WebSockets;

namespace WebsocketBase.Handlers.Proxy;

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
			try
			{
				await ProxyCurrencyRates(webSocket, cts.Token);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error during WebSocket handling: {ex.Message}");
				if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
				{
					await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Internal server error", CancellationToken.None);
				}
			}

			cts.Cancel();
			_connections.Remove(webSocket);
			Console.WriteLine($"User disconnected. Total users: {_connections.Count}");

			if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
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
		try
		{
			await serverWebSocket.ConnectAsync(new Uri("ws://localhost:8080"), token);
		}
		catch (WebSocketException ex)
		{
			Console.WriteLine($"Failed to connect to ws://localhost:8080: {ex.Message}");
			if (clientWebSocket.State == WebSocketState.Open || clientWebSocket.State == WebSocketState.CloseReceived)
			{
				await clientWebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Unable to connect to the remote server", CancellationToken.None);
			}
			return;
		}

		var buffer = new byte[1024];
		var receiveFromServerTask = Task.Run(async () =>
		{
			while (serverWebSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
			{
				try
				{
					Array.Clear(buffer, 0, buffer.Length);
					var result = await serverWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
					if (result.MessageType == WebSocketMessageType.Text)
					{
						if (clientWebSocket.State == WebSocketState.Open)
						{
							await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), WebSocketMessageType.Text, true, token);
							Console.WriteLine("Message forwarded to client.");
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
		}, token);

		var receiveFromClientTask = Task.Run(async () =>
		{
			while (clientWebSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
			{
				try
				{
					// Remove Array.Clear(buffer, 0, buffer.Length);
					var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
					if (result.MessageType == WebSocketMessageType.Text)
					{
						Console.WriteLine($"Received message from client: {System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count)}");
						if (serverWebSocket.State == WebSocketState.Open)
						{
							await serverWebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), WebSocketMessageType.Text, true, token);
							Console.WriteLine("Message forwarded to server.");
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
		}, token);

		await Task.WhenAny(receiveFromServerTask, receiveFromClientTask);
	}
}
