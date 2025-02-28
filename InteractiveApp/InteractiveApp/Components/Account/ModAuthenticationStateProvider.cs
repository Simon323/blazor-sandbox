using InteractiveApp.Client.Extensions;
using InteractiveApp.Client.Interfaces;
using InteractiveApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace InteractiveApp.Components.Account;

public class ModAuthenticationStateProvider : AuthenticationStateProvider, IClaimPrincipailSync
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IUserRequirementsService _userRequirementsService;

	private AuthenticationState? _authenticationState;
	public AuthenticationState? AuthenticationState => _authenticationState;

	public ModAuthenticationStateProvider(
		IHttpContextAccessor httpContextAccessor, IUserRequirementsService userRequirementsService)
	{
		_httpContextAccessor = httpContextAccessor;
		_userRequirementsService = userRequirementsService;
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		// Uzyskaj aktualny HttpContext
		var httpContext = _httpContextAccessor.HttpContext;
		ClaimsPrincipal user;

		// Jeśli HttpContext istnieje oraz użytkownik jest uwierzytelniony – używamy jego tożsamości
		if (httpContext != null && httpContext.User?.Identity != null && httpContext.User.Identity.IsAuthenticated)
		{
			//user = httpContext.User;
			// Utwórz kopię istniejącej tożsamości
			var identity = new ClaimsIdentity(httpContext.User.Identity);

			// Dodaj dodatkowe claimy – mogą to być np. dane pobrane z bazy lub inne informacje
			identity.AddClaim(new Claim("CustomClaim1", "Wartość1"));
			identity.AddClaim(new Claim("CustomClaim2", "Wartość2"));

			user = new ClaimsPrincipal(identity);
		}
		else
		{
			// Jeśli nie – tworzymy anonimowego użytkownika
			user = new ClaimsPrincipal(new ClaimsIdentity());
		}

		return Save(new AuthenticationState(user)); //Task.FromResult(new AuthenticationState(user));
	}

	private AuthenticationState Save(AuthenticationState state)
	{
		_authenticationState = state;
		return state;
	}

	public async Task RefreshUserRolesAsync()
	{
		if (_authenticationState is not null)
		{
			var currentPrincipal = _authenticationState.User;
			var userId = currentPrincipal.GetUserId();
			if (userId is not null)
			{
				var roles = await _userRequirementsService.GetUserRoles(userId);
				var currentIdentity = currentPrincipal.Identity as ClaimsIdentity;

				if (currentIdentity == null)
				{
					currentIdentity = new ClaimsIdentity();
				}

				var existingClaims = UpdateClaims(currentIdentity.Claims.ToList(), roles);
				var updatedIdentity = new ClaimsIdentity(existingClaims, currentIdentity.AuthenticationType);
				var newPrincipal = new ClaimsPrincipal(updatedIdentity);

				_authenticationState = new AuthenticationState(newPrincipal);
				Save(_authenticationState);
				NotifyAuthenticationStateChanged(Task.FromResult(_authenticationState));
			}
		}
	}

	private static List<Claim> UpdateClaims(List<Claim> existingClaims, List<string> roles)
	{
		var userRoles = roles;

		existingClaims.RemoveAll(c => c.Type == ClaimTypes.Role);

		var newRoleClaims = userRoles.Distinct().Select(role => new Claim(ClaimTypes.Role, role));
		existingClaims.AddRange(newRoleClaims);

		return existingClaims;
	}
}
