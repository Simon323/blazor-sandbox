using InteractiveApp.Client;
using InteractiveApp.Client.Extensions;
using InteractiveApp.Client.Interfaces;
using InteractiveApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;

namespace InteractiveApp.Components.Account;

public class ModAuthenticationStateProvider : AuthenticationStateProvider, IClaimPrincipailSync, IDisposable
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IUserRequirementsService _userRequirementsService;

	private AuthenticationState? _authenticationState;
	public AuthenticationState? AuthenticationState => _authenticationState;

	// Persistent
	private readonly PersistingComponentStateSubscription subscription;
	private readonly PersistentComponentState state;
	private Task<AuthenticationState>? authenticationStateTask;
	private readonly IdentityOptions options;

	public ModAuthenticationStateProvider(
		IHttpContextAccessor httpContextAccessor,
		IUserRequirementsService userRequirementsService,
		PersistentComponentState persistentComponentState,
		IOptions<IdentityOptions> optionsAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
		_userRequirementsService = userRequirementsService;

		state = persistentComponentState;
		options = optionsAccessor.Value;
		AuthenticationStateChanged += OnAuthenticationStateChanged;
		subscription = state.RegisterOnPersisting(OnPersistingAsync, Microsoft.AspNetCore.Components.Web.RenderMode.InteractiveWebAssembly);
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		var httpContext = _httpContextAccessor.HttpContext;
		ClaimsPrincipal user;

		if (httpContext != null && httpContext.User?.Identity != null && httpContext.User.Identity.IsAuthenticated)
		{
			var identity = new ClaimsIdentity(httpContext.User.Identity);

			identity.AddClaim(new Claim("CustomClaim1", "Wartość1"));
			identity.AddClaim(new Claim("CustomClaim2", "Wartość2"));

			user = new ClaimsPrincipal(identity);
		}
		else
		{
			user = new ClaimsPrincipal(new ClaimsIdentity());
		}

		return Save(new AuthenticationState(user));
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

	// WASM
	private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
	{
		authenticationStateTask = task;
	}

	private async Task OnPersistingAsync()
	{
		var principal = _authenticationState.User;

		if (principal.Identity?.IsAuthenticated == true)
		{
			var userId = principal.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;
			var email = principal.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;

			if (userId != null && email != null)
			{
				var roles = principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

				state.PersistAsJson(nameof(UserInfo), new UserInfo
				{
					UserId = userId,
					Email = email,
					Roles = roles
				});
			}
		}
	}

	//OLD
	private async Task OnPersistingAsyncOLD()
	{
		if (authenticationStateTask is null)
		{
			throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
		}

		var authenticationState = await authenticationStateTask;
		var principal = authenticationState.User;

		if (principal.Identity?.IsAuthenticated == true)
		{
			var userId = principal.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;
			var email = principal.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;

			if (userId != null && email != null)
			{
				state.PersistAsJson(nameof(UserInfo), new UserInfo
				{
					UserId = userId,
					Email = email,
					Roles = []
				});
			}
		}
	}

	public void Dispose()
	{
		subscription.Dispose();
		AuthenticationStateChanged -= OnAuthenticationStateChanged;
	}
}
