using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MultitenantApp.Data;

public class AppDbContext : MultiTenantDbContext
{
	public DbSet<Product> Products { get; set; }

	public DbSet<User> Users { get; set; }

	// these constructors are called when dependency injection is used
	public AppDbContext(IMultiTenantContextAccessor multiTenantContextAccessor)
		: base(multiTenantContextAccessor)
	{
	}

	public AppDbContext(IMultiTenantContextAccessor multiTenantContextAccessor, DbContextOptions<AppDbContext> options) :
		base(multiTenantContextAccessor, options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Product>().IsMultiTenant();

		modelBuilder.Entity<SeedProduct>().HasData(
			new { Id = 1, Name = "iPhone", TenantId = "1" }, // Apple
			new { Id = 2, Name = "MacBook", TenantId = "1" },
			new { Id = 3, Name = "Galaxy S21", TenantId = "2" }, // Samsung
			new { Id = 4, Name = "Galaxy Tab", TenantId = "2" }
		);

		modelBuilder
			.Entity<User>()
			.HasData(
				new User { Id = 1, Name = "John", Surname = "Doe" },
				new User { Id = 2, Name = "Jay", Surname = "Garrick" },
				new User { Id = 3, Name = "Barry", Surname = "Allen" },
				new User { Id = 4, Name = "Wally", Surname = "West" },
				new User { Id = 5, Name = "Bart", Surname = "Allen" }
			);

		base.OnModelCreating(modelBuilder);
	}
}