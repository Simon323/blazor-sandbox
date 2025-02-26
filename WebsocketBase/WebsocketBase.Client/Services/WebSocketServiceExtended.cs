using System.Net.WebSockets;
using System.Text;

namespace WebsocketBase.Client.Services;

/// <summary>
/// Usługa obsługująca trwałe połączenie WebSocket.
/// Zarejestrowana jako singleton, aby połączenie było współdzielone między stronami.
/// </summary>
public class WebSocketServiceExtended : IAsyncDisposable
{
	private ClientWebSocket _socket;
	private CancellationTokenSource _cts;
	private readonly Uri _serverUri;
	private readonly TimeSpan _reconnectDelay = TimeSpan.FromSeconds(5);

	// Zdarzenia pozwalające komponentom reagować na zmiany.
	public event Action<string> OnMessageReceived;
	public event Action OnConnected;
	public event Action OnDisconnected;

	// Właściwość pomocnicza informująca o stanie połączenia.
	public bool IsConnected => _socket != null && _socket.State == WebSocketState.Open;

	public WebSocketServiceExtended()
	{
		// Ustaw adres serwera WebSocket (tu przykładowy echo server; zmień na własny endpoint).
		_serverUri = new Uri("wss://echo.websocket.org");
		_cts = new CancellationTokenSource();
	}

	/// <summary>
	/// Rozpocznij połączenie WebSocket i nasłuchiwanie komunikatów w tle.
	/// Wywołaj tę metodę raz (np. podczas startu aplikacji).
	/// </summary>
	public void Start()
	{
		if (_socket != null && _socket.State == WebSocketState.Open)
			return; // już połączone

		// Tworzymy nowe połączenie WebSocket i uruchamiamy pętlę nasłuchiwania w tle.
		_socket = new ClientWebSocket();
		// Uruchamiamy asynchronicznie pętlę obsługi połączenia (bez czekania na jej zakończenie).
		_ = Task.Run(RunConnectionLoopAsync);
	}

	/// <summary>
	/// Główna pętla utrzymująca połączenie. 
	/// Łączy z serwerem i nasłuchuje wiadomości. W razie rozłączenia ponawia próby połączenia.
	/// </summary>
	private async Task RunConnectionLoopAsync()
	{
		// Dopóki aplikacja nie została zamknięta (token nie anulowany) – utrzymuj/odnawiaj połączenie.
		while (!_cts.IsCancellationRequested)
		{
			try
			{
				// Jeśli nie jesteśmy połączeni, podejmij próbę połączenia.
				if (_socket == null || _socket.State != WebSocketState.Open)
				{
					// Upewnij się, że stary socket jest zamknięty i zwolniony
					_socket?.Dispose();
					_socket = new ClientWebSocket();
					try
					{
						// Próba nawiązania połączenia z serwerem
						await _socket.ConnectAsync(_serverUri, _cts.Token);
						OnConnected?.Invoke(); // zgłoś zdarzenie o ustanowieniu połączenia
					}
					catch (Exception ex)
					{
						// Nie udało się połączyć (np. brak sieci lub serwer niedostępny).
						// Zgłoś rozłączenie i odczekaj przed ponowieniem próby.
						OnDisconnected?.Invoke();
#if DEBUG
						Console.WriteLine($"WebSocket connect error: {ex.Message}");
#endif
						// Odczekaj określony czas przed ponowną próbą połączenia.
						try
						{
							await Task.Delay(_reconnectDelay, _cts.Token);
						}
						catch (TaskCanceledException) { /* Ignoruj anulowanie */ }
						// Przejdź do kolejnej iteracji pętli (ponowna próba).
						continue;
					}
				}

				// Jesteśmy połączeni – odbieraj wiadomości w pętli.
				var buffer = new byte[4096];
				WebSocketReceiveResult result = null;
				var messageText = string.Empty;
				// Używamy MemoryStream do złożenia pełnej wiadomości (gdyby była podzielona na fragmenty).
				using (var ms = new System.IO.MemoryStream())
				{
					do
					{
						// Czekaj na fragment danych z serwera
						result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
						if (result.MessageType == WebSocketMessageType.Close)
						{
							// Serwer zamknął połączenie:
							break;
						}
						// Zapisz odebrane bajty do strumienia
						ms.Write(buffer, 0, result.Count);
					}
					while (!result.EndOfMessage); // pętla aż odbierzemy całą wiadomość

					if (result.MessageType == WebSocketMessageType.Close)
					{
						// Jeśli serwer zamknął połączenie, zamknij gniazdo lokalnie i zgłoś rozłączenie.
						await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
						OnDisconnected?.Invoke();
					}
					else
					{
						// Przekształć zebrane bajty na tekst (zakładamy komunikaty tekstowe w UTF8).
						ms.Seek(0, System.IO.SeekOrigin.Begin);
						messageText = Encoding.UTF8.GetString(ms.ToArray());
					}
				}

				// Jeśli odebrano tekst, przekaż go dalej przez zdarzenie.
				if (!string.IsNullOrEmpty(messageText))
				{
					OnMessageReceived?.Invoke(messageText);
				}
			}
			catch (OperationCanceledException)
			{
				// Anulowano (najpewniej zamknięcie aplikacji) – wyjdź z pętli.
				break;
			}
			catch (Exception e)
			{
				// Wystąpił błąd podczas odbierania (np. zerwane połączenie).
				OnDisconnected?.Invoke();
#if DEBUG
				Console.WriteLine($"WebSocket receive error: {e.Message}");
#endif
				// Kontynuuj pętlę - spróbujemy ponownie połączyć.
			}

			// Jeśli połączenie zostało przerwane, odczekaj chwilę przed ponowną próbą (zapobieganie szybkiemu zapętleniu).
			if (!_cts.IsCancellationRequested && (_socket == null || _socket.State != WebSocketState.Open))
			{
				try
				{
					await Task.Delay(_reconnectDelay, _cts.Token);
				}
				catch (TaskCanceledException) { /* Ignoruj anulowanie */ }
			}
		}
	}

	/// <summary>
	/// Wysyła komunikat tekstowy przez WebSocket do serwera.
	/// </summary>
	public async Task SendAsync(string message)
	{
		if (!IsConnected) return;
		var bytes = Encoding.UTF8.GetBytes(message);
		var segment = new ArraySegment<byte>(bytes);
		await _socket.SendAsync(segment, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: CancellationToken.None);
	}

	/// <summary>
	/// Zatrzymuje usługę i zamyka połączenie WebSocket.
	/// </summary>
	public async Task StopAsync()
	{
		// Anuluj pętlę i zamknij połączenie, jeśli aktywne.
		_cts.Cancel();
		if (_socket != null && _socket.State == WebSocketState.Open)
		{
			await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Application stopping", CancellationToken.None);
		}
		_socket?.Dispose();
	}

	/// <summary>
	/// Implementacja IAsyncDisposable – zapewnia zwolnienie zasobów przy zamykaniu aplikacji.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		// Anuluj pętlę nasłuchującą i zamknij gniazdo
		_cts.Cancel();
		if (_socket != null && _socket.State == WebSocketState.Open)
		{
			try
			{
				await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "App disposed", CancellationToken.None);
			}
			catch { /* Ignoruj ewentualne błędy przy zamykaniu */ }
		}
		_socket?.Dispose();
	}
}
