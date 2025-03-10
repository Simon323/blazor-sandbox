using Finbuckle.MultiTenant.Abstractions;
using MultitenantApp.Data;

namespace MultitenantApp.Strategies;

public class MyCustomTenantStrategy : IMultiTenantStrategy
{
	private readonly AppDbContext _db;

	public MyCustomTenantStrategy(AppDbContext db)
	{
		_db = db;
	}

	public async Task<string?> GetIdentifierAsync(object context)
	{
		var users = _db.Users.ToList();
		if (context is HttpContext httpContext)
		{
			if (httpContext.Request.Headers.TryGetValue("tenant", out var tenantId))
			{
				return tenantId.ToString();
			}
		}

		return null;
	}
}

