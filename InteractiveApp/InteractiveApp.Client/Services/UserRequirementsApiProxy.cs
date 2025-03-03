using InteractiveApp.Client.Interfaces;

namespace InteractiveApp.Client.Services;

public class UserRequirementsApiProxy : IUserRequirementsService
{
	private readonly IUserRequirementsApi _userRequirementsApi;

	public UserRequirementsApiProxy(IUserRequirementsApi userRequirementsApi)
	{
		_userRequirementsApi = userRequirementsApi;
	}

	public async Task<List<string>> GetUserRoles(string userId) =>
		await _userRequirementsApi.GetUserRoles();
}
