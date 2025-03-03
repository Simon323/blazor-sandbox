namespace InteractiveApp.Client.Interfaces;

public interface IUserRequirementsService
{
	Task<List<string>> GetUserRoles(string userId);
}
