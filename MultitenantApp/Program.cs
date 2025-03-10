using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using MultitenantApp.Data;
using MultitenantApp.Strategies;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
	//options.UseInMemoryDatabase("InMemoryDb");
	options.UseSqlServer(connectionString);
}, ServiceLifetime.Transient);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMultiTenant<TenantInfo>()
	//.WithHeaderStrategy("tenant")
	.WithStrategy<MyCustomTenantStrategy>(ServiceLifetime.Scoped)
	.WithInMemoryStore(options =>
	{
		options.Tenants.Add(new TenantInfo { Id = "1", Identifier = "Apple" });
		options.Tenants.Add(new TenantInfo { Id = "2", Identifier = "Samsung" });
	});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	dbContext.Database.EnsureCreated(); // Tworzy baz�, je�li nie istnieje
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMultiTenant();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();