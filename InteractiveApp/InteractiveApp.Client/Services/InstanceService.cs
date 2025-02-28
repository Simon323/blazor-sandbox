namespace InteractiveApp.Client.Services;

// IInstanceService.cs
public interface IInstanceService
{
	Task<List<Instance>> GetInstancesByCompanyAsync(int companyId);
}

// InstanceService.cs
public class InstanceService : IInstanceService
{
	public async Task<List<Instance>> GetInstancesByCompanyAsync(int companyId)
	{
		// Tu również pobierasz dane z bazy/API, przykładowe na sztywno:
		await Task.Delay(100);
		// Zwracamy różne instancje w zależności od ID firmy
		return companyId switch
		{
			1 => new List<Instance>
		{
			new Instance { Id = 101, Name = "Instancja A1" },
			new Instance { Id = 102, Name = "Instancja A2" },
		},
			2 => new List<Instance>
		{
			new Instance { Id = 201, Name = "Instancja B1" },
		},
			3 => new List<Instance>
		{
			new Instance { Id = 301, Name = "Instancja C1" },
			new Instance { Id = 302, Name = "Instancja C2" },
			new Instance { Id = 303, Name = "Instancja C3" },
		},
			_ => new List<Instance>()
		};
	}
}

// Przykładowa klasa modeli:
public class Instance
{
	public int Id { get; set; }
	public string Name { get; set; }
}
