namespace MultitenantApp.Data;

public class Product
{
	public int Id { get; set; }
	public string Name { get; set; }
}

public class SeedProduct : Product
{
	public string TenantId { get; set; }
}
