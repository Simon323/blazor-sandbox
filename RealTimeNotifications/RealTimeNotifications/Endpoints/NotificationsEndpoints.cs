using Microsoft.AspNetCore.SignalR;
using RealTimeNotifications.Hubs;
using RealTimeNotifications.Shared;

namespace RealTimeNotifications.Endpoints;

public static class NotificationsEndpoints
{
	const string GetEndpointName = "Notifications";

	public static RouteGroupBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/notifications");

		group.MapGet("", async (IHubContext<TestHub, ITestHub> _hubContext) =>
		{
			await _hubContext.Clients.All.ReceiveNotification("To all");
			return Results.Ok();
		}).WithName(GetEndpointName);

		return group;
	}
}
