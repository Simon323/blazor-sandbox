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
	private bool _isConnected = false;

	public event Action<List<CurrencyRate>> OnRatesUpdated;
	public event Action OnDisconnected;
	public bool IsConnected => _isConnected;

	public WebSocketService()
	{
		_serverUri = new Uri("ws://localhost:8080");
	}

	public async Task ConnectAsync()
	{
		if (_isConnected) return;

		_webSocket = new ClientWebSocket();
		_cts = new CancellationTokenSource();
		try
		{
			await _webSocket.ConnectAsync(_serverUri, _cts.Token);
			_isConnected = true;
			_ = ReceiveLoopAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Błąd WebSocket: {ex.Message}");
			_isConnected = false;
		}
	}

	private async Task ReceiveLoopAsync()
	{
		var buffer = new byte[1024];

		try
		{
			while (_isConnected && _webSocket.State == WebSocketState.Open)
			{
				var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
				if (result.MessageType == WebSocketMessageType.Close) break;

				var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
				var rates = JsonSerializer.Deserialize<List<CurrencyRate>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				OnRatesUpdated?.Invoke(rates);
			}
		}
		catch (OperationCanceledException)
		{
			Console.WriteLine("WebSocket zamknięty przez użytkownika.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Błąd odbioru WebSocket: {ex.Message}");
		}
		finally
		{
			await DisconnectAsync(); // Bezpieczne zamknięcie
		}
	}

	public async Task DisconnectAsync()
	{
		if (!_isConnected) return;

		_isConnected = false;
		_cts?.Cancel();

		try
		{
			if (_webSocket != null && _webSocket.State == WebSocketState.Open)
			{
				await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "User disconnected", CancellationToken.None);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Błąd przy zamykaniu WebSocket: {ex.Message}");
		}
		finally
		{
			_webSocket?.Dispose();
			_webSocket = null;
			OnDisconnected?.Invoke();
		}
	}

	public async ValueTask DisposeAsync()
	{
		await DisconnectAsync();
	}
}