using Microsoft.AspNetCore.Mvc;
using MultitenantApp.Data;

namespace MultitenantApp.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : Controller
{
	private readonly AppDbContext _db;

	public UsersController(AppDbContext db)
	{
		_db = db;
	}
	// GET
	[HttpGet]
	public IActionResult GetAll()
	{
		return Ok(_db.Users.ToList());
	}

	[HttpGet("{id}")]
	public IActionResult Get(int id)
	{
		return Ok(_db.Users.FirstOrDefault(x => x.Id == id));
	}

	[HttpPost]
	public IActionResult Create(User product)
	{
		_db.Users.Add(product);
		_db.SaveChanges();
		return Ok(product);
	}

	[HttpDelete("{id}")]
	public IActionResult Delete(int id)
	{
		_db.Users.Remove(_db.Users.FirstOrDefault(x => x.Id == id));
		_db.SaveChanges();
		return NoContent();
	}
}
