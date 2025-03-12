using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using Microsoft.EntityFrameworkCore;

namespace MultiAppV2.Data;

public class MultiTenantStoreDbContext : EFCoreStoreDbContext<TenantInfo>
{
	public MultiTenantStoreDbContext(DbContextOptions<MultiTenantStoreDbContext> options)
		: base(options) { }
}