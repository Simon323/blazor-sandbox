using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiAppV2.Data;
using MultiAppV2.Models;

namespace MultiAppV2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
	private readonly AppDbContext _context;
	public ProductsController(AppDbContext context)
	{
		_context = context;
	}

	// GET: api/Products
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
	{
		var products = await _context.Products.ToListAsync();
		return Ok(products);
	}

	// POST: api/Products
	[HttpPost]
	public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
	{
		_context.Products.Add(product);
		await _context.SaveChangesAsync();
		return CreatedAtAction(nameof(GetAllProducts), new { id = product.Id }, product);
	}
}