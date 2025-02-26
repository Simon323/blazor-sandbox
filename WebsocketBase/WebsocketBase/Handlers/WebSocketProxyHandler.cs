using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace WebsocketBase.Handlers;

public class WebSocketProxyHandler
{
	private static readonly ConcurrentDictionary<WebSocket, bool> _clients = new();
	private readonly ClientWebSocket _externalWebSocket = new();
	private readonly Uri _externalWebSocketUri = new("ws://localhost:8080");
	private CancellationTokenSource _cts = new();

	public WebSocketProxyHandler()
	{
		_ = ConnectToExternalWebSocketAsync();
	}

	public async Task HandleWebSocketAsync(HttpContext context)
	{
		if (context.WebSockets.IsWebSocketRequest)
		{
			using WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
			_clients.TryAdd(webSocket, true);

			Console.WriteLine("🟢 Nowy klient połączony!");

			try
			{
				await ForwardExternalWebSocketData(webSocket);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ WebSocket błąd: {ex.Message}");
			}
			finally
			{
				_clients.TryRemove(webSocket, out _);
				await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
				Console.WriteLine("🔴 Klient rozłączony.");
			}
		}
		else
		{
			context.Response.StatusCode = 400;
		}
	}

	// 🔹 Połączenie do zewnętrznego WebSocket
	private async Task ConnectToExternalWebSocketAsync()
	{
		while (!_cts.Token.IsCancellationRequested)
		{
			try
			{
				await _externalWebSocket.ConnectAsync(_externalWebSocketUri, _cts.Token);
				Console.WriteLine($"✅ Połączono z {_externalWebSocketUri}!");

				await ReceiveExternalDataAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Błąd połączenia do {_externalWebSocketUri}: {ex.Message}");
				await Task.Delay(5000); // Ponów próbę połączenia po 5 sekundach
			}
		}
	}

	// 🔹 Odbieranie danych z zewnętrznego WebSocket i przekazywanie do klientów
	private async Task ReceiveExternalDataAsync()
	{
		var buffer = new byte[1024];

		while (_externalWebSocket.State == WebSocketState.Open)
		{
			var result = await _externalWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			if (result.MessageType == WebSocketMessageType.Close)
			{
				break;
			}

			var data = Encoding.UTF8.GetString(buffer, 0, result.Count);
			await BroadcastToClients(data);
		}
	}

	// 🔹 Przesyłanie danych do wszystkich klientów
	private async Task BroadcastToClients(string message)
	{
		var buffer = Encoding.UTF8.GetBytes(message);

		foreach (var client in _clients.Keys)
		{
			if (client.State == WebSocketState.Open)
			{
				await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
			}
		}
	}

	// 🔹 Przekazywanie danych klientowi WebSocket w API
	private async Task ForwardExternalWebSocketData(WebSocket webSocket)
	{
		var buffer = new byte[1024];

		while (webSocket.State == WebSocketState.Open)
		{
			var result = await _externalWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			if (result.MessageType == WebSocketMessageType.Close) break;

			await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), WebSocketMessageType.Text, true, CancellationToken.None);
		}
	}
}