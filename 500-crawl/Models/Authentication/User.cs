namespace _500_crawl.Authentication;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public int? GameId { get; set; } = null;
}