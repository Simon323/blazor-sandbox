using InteractiveApp.Client.Interfaces;

namespace InteractiveApp.Client.Services;

public class IdentityUserApiProxy : IIdentityUserService
{
	private readonly IIdentityUserApi _identityUserApi;

	public IdentityUserApiProxy(IIdentityUserApi identityUserApi)
	{
		_identityUserApi = identityUserApi;
	}

	public Task AddRandomRoles(string userId) =>
		_identityUserApi.AddRandomRoles();

	public Task DeleteAllRoles(string userId) =>
		_identityUserApi.DeleteAllRoles();

	public Task Refresh(string userId)
	{
		throw new NotImplementedException();
	}
}
