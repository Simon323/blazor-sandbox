using InteractiveApp.Client.Interfaces;
using InteractiveApp.Components.Account;
using InteractiveApp.Data;
using Microsoft.AspNetCore.Identity;

namespace InteractiveApp.Services;

public class IdentityUserService : IIdentityUserService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IServiceScopeFactory scopeFactory;
	private readonly SignInManager<ApplicationUser> signInManager;
	private readonly ModAuthenticationStateProvider _modAuthenticationStateProvider;

	public IdentityUserService(UserManager<ApplicationUser> userManager, IServiceScopeFactory scopeFactory, SignInManager<ApplicationUser> signInManager, ModAuthenticationStateProvider modAuthenticationStateProvider)
	{
		_userManager = userManager;
		this.scopeFactory = scopeFactory;
		this.signInManager = signInManager;
		_modAuthenticationStateProvider = modAuthenticationStateProvider;
	}

	public async Task AddRandomRoles(string userId)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
		{
			throw new ArgumentException("User not found");
		}

		var roles = new List<string> { "Admin", "User", "Moderator" };
		var random = new Random();
		var randomRoles = roles.OrderBy(x => random.Next()).Take(2).ToList();

		foreach (var role in randomRoles)
		{
			if (!await _userManager.IsInRoleAsync(user, role))
			{
				await _userManager.AddToRoleAsync(user, role);
			}
		}
		//await Refresh(userId);
	}

	public async Task DeleteAllRoles(string userId)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
		{
			throw new ArgumentException("User not found");
		}
		var roles = await _userManager.GetRolesAsync(user);
		foreach (var role in roles)
		{
			await _userManager.RemoveFromRoleAsync(user, role);
		}

		//await Refresh(userId);
	}

	//private async Task Refresh(string userId)
	//{
	//	await using var scope = scopeFactory.CreateAsyncScope();
	//	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
	//	var user = await userManager.FindByIdAsync(userId);
	//	await signInManager.RefreshSignInAsync(user);
	//}

	public async Task Refresh(string userId)
	{
		await _modAuthenticationStateProvider.RefreshUserRolesAsync();
	}
}
