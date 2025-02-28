using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace InteractiveApp.Components.Account;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public override Task<AuthenticationState> GetAuthenticationStateAsync()
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

		return Task.FromResult(new AuthenticationState(user));
	}

	//public async Task RefreshUserRolesAsync()
	//{
	//	if (_authenticationState is not null)
	//	{
	//		var currentPrincipal = _authenticationState.User;
	//		var userId = currentPrincipal.GetUserId();
	//		if (userId is not null)
	//		{
	//			var result = await _roleProvider.GetUserRoles(userId);
	//			if (result.Success && result.Data is not null)
	//			{
	//				var currentIdentity = currentPrincipal.Identity as ClaimsIdentity;

	//				if (currentIdentity == null)
	//				{
	//					currentIdentity = new ClaimsIdentity();
	//				}

	//				var existingClaims = UpdateClaims(currentIdentity.Claims.ToList(), result.Data);
	//				var updatedIdentity = new ClaimsIdentity(existingClaims, currentIdentity.AuthenticationType);
	//				var newPrincipal = new ClaimsPrincipal(updatedIdentity);

	//				_authenticationState = new AuthenticationState(newPrincipal);
	//				Save(_authenticationState);
	//				NotifyAuthenticationStateChanged(Task.FromResult(_authenticationState));
	//			}
	//		}
	//	}
	//}
}
