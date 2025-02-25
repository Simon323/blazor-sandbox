namespace RealTimeNotifications.Shared.Interfaces;

public interface ITestHub
{
	Task ToGroup(string group);
	Task JoinGroup(string group);
	Task ReceiveNotification(string message);
}
