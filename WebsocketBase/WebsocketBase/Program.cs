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

app.Run();
