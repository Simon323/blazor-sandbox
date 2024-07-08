namespace ServerManagement.Frontend.Models;

public static class CitiesRepository
{
	private static List<string> Cities = new List<string>()
	{
		"New York",
		"Los Angeles",
		"Chicago",
		"Houston",
		"Phoenix",
		"Philadelphia",
		"San Antonio",
		"San Diego",
		"Dallas"
	};

	public static List<string> GetCities() => Cities;
}
