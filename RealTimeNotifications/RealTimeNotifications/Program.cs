using Radzen;
using RealTimeNotifications.Client.Services;
using RealTimeNotifications.Components;
using RealTimeNotifications.Endpoints;
using RealTimeNotifications.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<SignalRService>();

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
	.AddAdditionalAssemblies(typeof(RealTimeNotifications.Client._Imports).Assembly);

app.MapHub<TestHub>("/testhub");
app.MapHub<RetroHub>("/retrohub");
app.MapNotificationsEndpoints();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Game Store API");
	c.EnableTryItOutByDefault();
});

app.Run();
