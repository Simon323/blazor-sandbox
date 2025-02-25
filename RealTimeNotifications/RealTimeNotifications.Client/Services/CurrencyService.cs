using RealTimeNotifications.Shared.Messages;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace RealTimeNotifications.Client.Services;

public class CurrencyService : IAsyncDisposable
{
	private ClientWebSocket _webSocket;
	private CancellationTokenSource _cts;
	public event Action<List<CurrencyRate>> OnRatesUpdated;

	public async Task ConnectAsync()
	{
		_webSocket = new ClientWebSocket();
		_cts = new CancellationTokenSource();
		await _webSocket.ConnectAsync(new Uri("ws://localhost:8080"), _cts.Token);
		_ = ReceiveLoopAsync();
	}

	private async Task ReceiveLoopAsync()
	{
		var buffer = new byte[1024];

		while (_webSocket.State == WebSocketState.Open)
		{
			var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
			if (result.MessageType == WebSocketMessageType.Close)
			{
				await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
				break;
			}

			var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
			Console.WriteLine($"Odebrany JSON: {json}"); // DEBUG JSON

			try
			{
				var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
				var rates = JsonSerializer.Deserialize<List<CurrencyRate>>(json, options);

				if (rates != null)
					OnRatesUpdated?.Invoke(rates);
				else
					Console.WriteLine("Deserializacja zwróciła null!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Błąd deserializacji: {ex.Message}");
			}
		}
	}

	public async ValueTask DisposeAsync()
	{
		_cts?.Cancel();
		_webSocket?.Dispose();
	}
}