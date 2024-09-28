using BlazorIdentityApp.Shared.Dto;
using BlazorIdentityApp.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlazorIdentityApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IUserAccount userAccount) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto userDTO)
    {
        var response = await userAccount.CreateAccount(userDTO);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDTO)
    {
        var response = await userAccount.LoginAccount(loginDTO);
        return Ok(response);
    }
}
