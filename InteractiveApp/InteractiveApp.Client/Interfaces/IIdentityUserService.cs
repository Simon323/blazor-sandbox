namespace InteractiveApp.Client.Interfaces;

public interface IIdentityUserService
{
	Task AddRandomRoles(string userId);
	Task DeleteAllRoles(string userId);
	Task Refresh(string userId);
}
