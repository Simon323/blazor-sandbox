namespace InteractiveApp.Client.Services;

// ICompanyService.cs
public interface ICompanyService
{
	Task<List<Company>> GetCompaniesAsync();
}

// CompanyService.cs
public class CompanyService : ICompanyService
{
	public async Task<List<Company>> GetCompaniesAsync()
	{
		// Tu pobierasz dane np. z bazy danych / API
		// Poniżej przykładowe dane na sztywno:
		await Task.Delay(100); // symulacja asynchroniczności
		return new List<Company>
		{
			new Company { Id = 1, Name = "Firma A" },
			new Company { Id = 2, Name = "Firma B" },
			new Company { Id = 3, Name = "Firma C" }
		};
	}
}

// Przykładowa klasa modeli:
public class Company
{
	public int Id { get; set; }
	public string Name { get; set; }
}

