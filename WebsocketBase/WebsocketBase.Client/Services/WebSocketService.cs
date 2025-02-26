using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebsocketBase.Shared.Messages;

namespace WebsocketBase.Client.Services;

public class WebSocketService : IAsyncDisposable
{
	private ClientWebSocket _webSocket;
	private CancellationTokenSource _cts;
	private readonly Uri _serverUri;
	public event Action<List<CurrencyRate>> OnRatesUpdated;
	public bool IsConnected => _webSocket?.State == WebSocketState.Open;

	public WebSocketService()
	{
		_serverUri = new Uri("ws://localhost:8080"); // Ustaw odpowiedni adres WebSocket
	}

	public async Task ConnectAsync()
	{
		if (IsConnected) return;

		_webSocket = new ClientWebSocket();
		_cts = new CancellationTokenSource();
		await _webSocket.ConnectAsync(_serverUri, _cts.Token);
		_ = ReceiveLoopAsync(); // Startuj w tle
	}

	private async Task ReceiveLoopAsync()
	{
		var buffer = new byte[1024];

		while (_webSocket.State == WebSocketState.Open)
		{
			var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
			if (result.MessageType == WebSocketMessageType.Close) break;

			var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
			var rates = JsonSerializer.Deserialize<List<CurrencyRate>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			OnRatesUpdated?.Invoke(rates);
		}
	}

	public async Task DisconnectAsync()
	{
		if (!IsConnected) return;

		_cts.Cancel();
		await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "User disconnected", CancellationToken.None);
		_webSocket.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		await DisconnectAsync();
	}
}
