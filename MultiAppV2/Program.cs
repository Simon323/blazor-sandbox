using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using MultiAppV2.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MultiTenantStoreDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(connectionString));

// Konfiguracja Finbuckle.MultiTenant
builder.Services.AddMultiTenant<TenantInfo>()
	.WithEFCoreStore<MultiTenantStoreDbContext, TenantInfo>()
	.WithHeaderStrategy("x-tenant-id");

var app = builder.Build();

app.UseRouting();
app.UseMultiTenant();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
