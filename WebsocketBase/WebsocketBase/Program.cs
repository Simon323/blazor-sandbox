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
builder.Services.AddSingleton<WebSocketProxyHandler>();
builder.Services.AddSingleton<WebSocketCurrencyBroadcastHandler>();
builder.Services.AddSingleton<WebSocketCurrencyIndividualHandler>();
builder.Services.AddSingleton<WebSocketChatHandler>();

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

app.Map("/proxy-ws", async (HttpContext context, WebSocketProxyHandler handler) =>
{
	await handler.HandleWebSocketAsync(context);
});

app.Map("/ws-currency", async (HttpContext context, WebSocketCurrencyBroadcastHandler handler) =>
{
	await handler.HandleWebSocketAsync(context);
});

app.Map("/ws-new", async (HttpContext context, WebSocketChatHandler handler) =>
{
	await handler.HandleWebSocketAsync(context);
});

app.Run();
