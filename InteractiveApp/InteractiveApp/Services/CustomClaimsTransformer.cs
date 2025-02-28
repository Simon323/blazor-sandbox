using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace InteractiveApp.Services;

public class CustomClaimsTransformer : IClaimsTransformation
{
	public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
	{
		Console.WriteLine("ClaimsTransformation !!!");
		var identity = (ClaimsIdentity)principal.Identity;

		if (!identity.HasClaim(c => c.Type == "CustomClaim"))
		{
			identity.AddClaim(new Claim("CustomClaim", "CustomValue"));
		}

		if (!identity.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "keke"))
		{
			identity.AddClaim(new Claim(ClaimTypes.Role, "keke"));
		}

		return Task.FromResult(principal);
	}
}
