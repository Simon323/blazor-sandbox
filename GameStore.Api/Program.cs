using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connectionString);
builder.Services.AddScoped<GameStoreContext>();

var app = builder.Build();

app.MapGamesEndpoints(); // Register the games endpoints
await app.MigrateDbAsync(); // Automatically migrate the database

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Game Store API");
	c.EnableTryItOutByDefault();
});

app.Run();