namespace ServerManagement.Frontend.Models;

public class Server
{
    public Server()
    {
        var random = new Random();
        int randomNumber = random.Next(0, 2);
        IsOnline = randomNumber == 1;
    }

    public int ServerId { get; set; }

    public bool IsOnline { get; set; }

    public string? Name { get; set; }
    
    public string? City { get; set; }
}
