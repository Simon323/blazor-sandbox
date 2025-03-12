using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiAppV2.Data;
using MultiAppV2.Models;

namespace MultiAppV2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly AppDbContext _context;
	public UsersController(AppDbContext context)
	{
		_context = context;
	}

	// GET: api/Users
	[HttpGet]
	public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
	{
		var users = await _context.Users.ToListAsync();
		return Ok(users);
	}

	// POST: api/Users
	[HttpPost]
	public async Task<ActionResult<User>> CreateUser([FromBody] User user)
	{
		_context.Users.Add(user);
		await _context.SaveChangesAsync();
		return CreatedAtAction(nameof(GetAllUsers), new { id = user.Id }, user);
	}
}