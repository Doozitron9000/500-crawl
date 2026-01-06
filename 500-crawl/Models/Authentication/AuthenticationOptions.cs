namespace _500_crawl.Authentication;

public class AuthenticationOptions
{
    // Store the pword pepper as a prop here to protect against typos etc causing us to fail
    // to get the pepper if we get it directly from the appsettings
    public string PasswordPepper { get; set; } = null!;
}