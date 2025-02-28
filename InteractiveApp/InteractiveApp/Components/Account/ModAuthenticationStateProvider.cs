using InteractiveApp.Client.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace InteractiveApp.Components.Account;

public class ModAuthenticationStateProvider : AuthenticationStateProvider
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	private AuthenticationState? _authenticationState;
	public AuthenticationState? AuthenticationState => _authenticationState;

	public ModAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
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
				var currentIdentity = currentPrincipal.Identity as ClaimsIdentity;

				if (currentIdentity == null)
				{
					currentIdentity = new ClaimsIdentity();
				}

				var existingClaims = UpdateClaims(currentIdentity.Claims.ToList());
				var updatedIdentity = new ClaimsIdentity(existingClaims, currentIdentity.AuthenticationType);
				var newPrincipal = new ClaimsPrincipal(updatedIdentity);

				_authenticationState = new AuthenticationState(newPrincipal);
				Save(_authenticationState);
				NotifyAuthenticationStateChanged(Task.FromResult(_authenticationState));
			}
		}
	}

	private static List<Claim> UpdateClaims(List<Claim> existingClaims)
	{
		if (!existingClaims.Any(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "nova"))
		{
			existingClaims.Add(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "nova"));
		}
		return existingClaims;
	}
}
