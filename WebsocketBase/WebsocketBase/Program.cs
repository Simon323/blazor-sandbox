using WebsocketBase.Client.Services;
using WebsocketBase.Components;
using WebsocketBase.Handlers.Local;
using WebsocketBase.Handlers.Proxy;
using WebsocketBase.Handlers.Sandbox;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddSingleton<CurrencyService>();
builder.Services.AddSingleton<WebSocketService>();
builder.Services.AddSingleton<WebSocketCurrencyBroadcastHandler>();
builder.Services.AddSingleton<WebSocketCurrencyIndividualHandler>();
builder.Services.AddSingleton<WebSocketChatHandler>();
builder.Services.AddSingleton<WebSocketCurrencyGroupProxyHandler>();
builder.Services.AddSingleton<WebSocketCurrencyIndividualProxyHandler>();

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

// Webapi provider websocket endpoint
app.Map("/ws-currency", async (HttpContext context, WebSocketCurrencyBroadcastHandler handler) =>
{
	await handler.HandleWebSocketAsync(context);
});

// All clients share the same connection to the server
app.Map("/ws-currency-group-proxy", async (HttpContext context, WebSocketCurrencyGroupProxyHandler handler) =>
{
	await handler.HandleWebSocketAsync(context);
});

// Each client has individual connection to the server
app.Map("/ws-currency-individual-proxy", async (HttpContext context, WebSocketCurrencyIndividualProxyHandler handler) =>
{
	await handler.HandleWebSocketAsync(context);
});

app.Map("/ws-new", async (HttpContext context, WebSocketChatHandler handler) =>
{
	await handler.HandleWebSocketAsync(context);
});

app.Run();
