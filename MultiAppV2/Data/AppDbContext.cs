using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiAppV2.Models;

namespace MultiAppV2.Data;

public class AppDbContext : MultiTenantDbContext
{
	public AppDbContext(IMultiTenantContextAccessor multiTenantContextAccessor)
		: base(multiTenantContextAccessor)
	{
	}

	public AppDbContext(IMultiTenantContextAccessor multiTenantContextAccessor, DbContextOptions<AppDbContext> options) :
		base(multiTenantContextAccessor, options)
	{
	}

	public DbSet<User> Users { get; set; }
	public DbSet<Product> Products { get; set; }
}
