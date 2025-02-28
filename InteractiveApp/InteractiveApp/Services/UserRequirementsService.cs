using InteractiveApp.Data;
using Microsoft.EntityFrameworkCore;

namespace InteractiveApp.Services;

public interface IUserRequirementsService
{
	Task<List<string>> GetUserRoles(string userId);
}

public class UserRequirementsService : IUserRequirementsService
{
	private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

	public UserRequirementsService(IDbContextFactory<ApplicationDbContext> contextFactory)
	{
		_contextFactory = contextFactory;
	}

	public async Task<List<string>> GetUserRoles(string userId)
	{
		await using var _applicationDbContext = _contextFactory.CreateDbContext();

		try
		{
			var userRolesResult = await (from u in _applicationDbContext.Users
										 join ur in _applicationDbContext.UserRoles on u.Id equals ur.UserId into userRoles
										 from ur in userRoles.DefaultIfEmpty()
										 join r in _applicationDbContext.Roles on ur.RoleId equals r.Id into roles2
										 from r in roles2.DefaultIfEmpty()
										 where u.Id == userId && r != null
										 select r != null ? r.Name : null).ToListAsync();




			if (userRolesResult is { Count: > 0 })
			{
				return new List<string>(userRolesResult);
			}
			return new List<string>();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return new List<string>();
		}
	}
}
