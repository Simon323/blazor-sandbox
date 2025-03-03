using Refit;

namespace InteractiveApp.Client.Services;

public interface IUserRequirementsApi
{
	[Get("/api/users/roles")]
	Task<List<string>> GetUserRoles();
}
