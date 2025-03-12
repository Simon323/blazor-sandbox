namespace MultiAppV2.Models;

using Finbuckle.MultiTenant;

[MultiTenant]
public class Product
{
	public int Id { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
}

