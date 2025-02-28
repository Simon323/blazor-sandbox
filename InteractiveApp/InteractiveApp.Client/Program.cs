using InteractiveApp.Client;
using InteractiveApp.Client.Interfaces;
using InteractiveApp.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddSingleton<PersistentAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<PersistentAuthenticationStateProvider>());
builder.Services.AddScoped<IClaimPrincipailSync>(provider => provider.GetRequiredService<PersistentAuthenticationStateProvider>());

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IInstanceService, InstanceService>();
builder.Services.AddScoped<AppState>();

builder.Services.AddTransient<IIdentityUserService, IdentityUserApiProxy>();

builder.Services.AddRefitClient<IIdentityUserApi>()
	.ConfigureHttpClient(httpClient =>
	{
		var apiUrl = builder.HostEnvironment.BaseAddress;
		httpClient.BaseAddress = new Uri(apiUrl);
	});

await builder.Build().RunAsync();
