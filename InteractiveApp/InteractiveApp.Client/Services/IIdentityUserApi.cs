using Refit;

namespace InteractiveApp.Client.Services;

public interface IIdentityUserApi
{
	[Post("/api/users/add-random-roles")]
	Task AddRandomRoles();

	[Delete("/api/users/delete-all-roles")]
	Task DeleteAllRoles();
}
