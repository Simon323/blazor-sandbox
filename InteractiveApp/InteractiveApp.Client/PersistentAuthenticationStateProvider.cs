using InteractiveApp.Client.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace InteractiveApp.Client
{
	// This is a client-side AuthenticationStateProvider that determines the user's authentication state by
	// looking for data persisted in the page when it was rendered on the server. This authentication state will
	// be fixed for the lifetime of the WebAssembly application. So, if the user needs to log in or out, a full
	// page reload is required.
	//
	// This only provides a user name and email for display purposes. It does not actually include any tokens
	// that authenticate to the server when making subsequent requests. That works separately using a
	// cookie that will be included on HttpClient requests to the server.
	internal class PersistentAuthenticationStateProvider : AuthenticationStateProvider, IClaimPrincipailSync
	{
		private readonly IUserRequirementsService _userRequirementsService;

		private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
			Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

		private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

		public PersistentAuthenticationStateProvider(PersistentComponentState state, IUserRequirementsService userRequirementsService)
		{
			if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
			{
				return;
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
				new Claim(ClaimTypes.Name, userInfo.Email),
				new Claim(ClaimTypes.Email, userInfo.Email)
			};

			if (userInfo.Roles is { Count: > 0 })
			{
				claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
			}

			authenticationStateTask = Task.FromResult(
				new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
					authenticationType: nameof(PersistentAuthenticationStateProvider)))));
			_userRequirementsService = userRequirementsService;
		}

		public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;

		public async Task RefreshUserRolesAsync()
		{
			var authenticationState = await authenticationStateTask;
			var principal = authenticationState.User;

			if (principal.Identity?.IsAuthenticated == true)
			{
				Console.WriteLine("RefreshUserRolesAsync !!!!");
				var roles = await _userRequirementsService.GetUserRoles(null);


				var currentIdentity = principal.Identity as ClaimsIdentity;
				if (currentIdentity == null)
				{
					currentIdentity = new ClaimsIdentity();
				}

				var existingClaims = UpdateClaims(currentIdentity.Claims.ToList(), roles);
				var updatedIdentity = new ClaimsIdentity(existingClaims, currentIdentity.AuthenticationType);
				var newPrincipal = new ClaimsPrincipal(updatedIdentity);
				var _authenticationState = new AuthenticationState(newPrincipal);
				NotifyAuthenticationStateChanged(Task.FromResult(_authenticationState));
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
}
