using RealTimeNotifications.Shared.Messages;

namespace RealTimeNotifications.Shared.Interfaces;

public interface IRetroHub
{
	Task ReceiveRetroItem(RetrospectiveItem item);
	Task UpdateClientsCount(int count);
}
