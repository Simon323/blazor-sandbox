using Microsoft.AspNetCore.SignalR;
using RealTimeNotifications.Shared.Interfaces;
using RealTimeNotifications.Shared.Messages;

namespace RealTimeNotifications.Hubs;

public class RetroHub : Hub<IRetroHub>
{
	static int clientsCount;

	public async Task SendRetroItem(RetrospectiveItem item)
	{
		await Clients.All.ReceiveRetroItem(item);
	}

	public override async Task OnConnectedAsync()
	{
		clientsCount++;
		await Clients.All.UpdateClientsCount(clientsCount);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		clientsCount--;
		await Clients.All.UpdateClientsCount(clientsCount);
		await base.OnDisconnectedAsync(exception);
	}
}
