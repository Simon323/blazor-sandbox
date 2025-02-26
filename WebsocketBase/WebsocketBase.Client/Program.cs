using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebsocketBase.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<CurrencyService>();

await builder.Build().RunAsync();
