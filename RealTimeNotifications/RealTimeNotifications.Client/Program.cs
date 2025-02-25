using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using RealTimeNotifications.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<NotificationService>();
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<SignalRService>();

await builder.Build().RunAsync();
