using Microsoft.AspNetCore.SignalR;
using RealTimeNotifications.Shared.Interfaces;

namespace RealTimeNotifications.Hubs;

public class TestHub : Hub<ITestHub>
{
	public async Task ToGroup(string group)
	{
		await Clients.Group(group).ReceiveNotification($"To group {group}");
	}

	public async Task JoinGroup(string group)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, group);
	}
}
