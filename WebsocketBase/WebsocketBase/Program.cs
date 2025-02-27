using System.Net;
using System.Net.WebSockets;
using System.Text;
using WebsocketBase.Client.Services;
using WebsocketBase.Components;
using WebsocketBase.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddSingleton<CurrencyService>();
builder.Services.AddSingleton<WebSocketService>();
builder.Services.AddSingleton<WebSocketHandler>();
builder.Services.AddSingleton<WebSocketProxyHandler>();
builder.Services.AddSingleton<WebSocketCurrencyHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(WebsocketBase.Client._Imports).Assembly);

app.UseWebSockets();

app.Map("/ws", async (HttpContext context, WebSocketHandler handler) =>
{
	await handler.HandleWebSocketAsync(context);
});

app.Map("/proxy-ws", async (HttpContext context, WebSocketProxyHandler handler) =>
{
	await handler.HandleWebSocketAsync(context);
});

app.Map("/ws-currency", async (HttpContext context, WebSocketCurrencyHandler handler) =>
{
	await handler.HandleWebSocketAsync(context);
});

var connections = new List<WebSocket>();
app.Map("/ws-new", async context =>
{
	if (context.WebSockets.IsWebSocketRequest)
	{
		var curName = context.Request.Query["name"];

		using var ws = await context.WebSockets.AcceptWebSocketAsync();

		connections.Add(ws);

		await Broadcast($"{curName} joined the room");
		await Broadcast($"{connections.Count} users connected");
		await ReceiveMessage(ws,
			async (result, buffer) =>
			{
				if (result.MessageType == WebSocketMessageType.Text)
				{
					string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
					await Broadcast(curName + ": " + message);
				}
				else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
				{
					connections.Remove(ws);
					await Broadcast($"{curName} left the room");
					await Broadcast($"{connections.Count} users connected");
					await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
				}
			});
	}
	else
	{
		context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
	}
});
async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
{
	var buffer = new byte[1024 * 4];
	while (socket.State == WebSocketState.Open)
	{
		var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		handleMessage(result, buffer);
	}
}

async Task Broadcast(string message)
{
	var bytes = Encoding.UTF8.GetBytes(message);
	foreach (var socket in connections)
	{
		if (socket.State == WebSocketState.Open)
		{
			var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
			await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
		}
	}
}

app.Run();
