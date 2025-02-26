using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebsocketBase.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<CurrencyService>();
builder.Services.AddSingleton<WebSocketService>();

await builder.Build().RunAsync();
