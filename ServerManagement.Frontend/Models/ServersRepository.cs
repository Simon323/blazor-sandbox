namespace ServerManagement.Frontend.Models;

public class ServersRepository
{
	private static List<Server> servers = new List<Server>
	{
		new Server { ServerId = 1, Name = "Dev-1", City = "New York" },
		new Server { ServerId = 2, Name = "Dev-2", City = "San Francisco" },
		new Server { ServerId = 3, Name = "Dev-3", City = "Chicago" },
		new Server { ServerId = 4, Name = "Dev-4", City = "Miami" },
		new Server { ServerId = 5, Name = "Dev-5", City = "Los Angeles" },
		new Server { ServerId = 6, Name = "Dev-6", City = "New York" },
		new Server { ServerId = 7, Name = "Dev-7", City = "Paris" },
		new Server { ServerId = 8, Name = "Dev-8", City = "London" },
		new Server { ServerId = 9, Name = "Dev-9", City = "Toronto" },
		new Server { ServerId = 10, Name = "Dev-10", City = "Buenos Aires" }
	};

	public static void Add(Server server)
	{
		server.ServerId = servers.Max(s => s.ServerId) + 1;
		servers.Add(server);
	}

	public static List<Server> GetServers() => servers;

	public static List<Server> GetServersByCity(string city) =>
		servers
			.Where(s =>
				s.City is not null
				&& s.City.Equals(city, StringComparison.OrdinalIgnoreCase))
			.ToList();

	public static Server GetServerById(int serverId)
	{
		var server = servers.Find(s => s.ServerId == serverId);
		if (server is null)
		{
			throw new InvalidOperationException($"Server with id {serverId} not found");
		}
		return server;
	}

	public static void UpdateServer(int serverId, Server updatedServer)
	{
		if (serverId != updatedServer.ServerId)
			throw new InvalidOperationException("Server id mismatch");

		var server = GetServerById(serverId);
		server.Name = updatedServer.Name;
		server.City = updatedServer.City;
		server.IsOnline = updatedServer.IsOnline;
	}

	public static void DeleteServer(int serverId)
	{
		var server = GetServerById(serverId);
		servers.Remove(server);
	}

	public static List<Server> SearchServers(string searchTerm)
	{
		return servers
			.Where(s =>
				s.Name is not null
				&& s.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
			.ToList();
	}
}
