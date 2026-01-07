using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _500_crawl.Models;
using _500_crawl.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace _500_crawl.Controllers;

public class LoginController : Controller
{
    // the database to which new users are sent and against which users are validated
    private readonly SiteDbContext database;
    // the password hasher
    private readonly IPasswordHasher<User> passwordHasher;
    // the authentication options
    private readonly AuthenticationOptions authOptions;

    public LoginController(SiteDbContext database, IPasswordHasher<User> passwordHasher, IOptions<AuthenticationOptions> authOptions)
    {   
        this.database = database;
        this.passwordHasher = passwordHasher;
        this.authOptions = authOptions.Value;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Signup()
    {
        return View();
    }

    /// <summary>
    /// Attempts to signup with the given user
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="password">The password</param>
    /// <param name="confPassword">The confirmed password</param>
    /// <returns>The action resulting from the signup attempt</returns>
    public IActionResult SignupAttempt(string username, string password, string confPassword)
    {
        // first make sure everything we got is valid
        if (string.IsNullOrWhiteSpace(password)) return BadRequest("No password given");
        if (password != confPassword) return BadRequest("Passwords do not match");
        if (string.IsNullOrWhiteSpace(username)) return BadRequest("No username given");

        // create a new user with the given user name
        User user = new User {Username = username};

        // Hash the password. The hasher should automatically salt the password.
        user.PasswordHash = passwordHasher.HashPassword(user, password+authOptions.PasswordPepper);

        try
        {
            database.Users.Add(user);
            database.SaveChanges();
        }
        catch (DbUpdateException)
        {
            // if we hit this the user already exists so return that
            return Conflict("Username already taken");
        }
        // if this was successful go to the login page
        return Ok();
    }

    public IActionResult Logout()
    {
        // just clear the session on logout
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Login");
    }

    /// <summary>
    /// Attempts to log the user in with the given credentials.
    /// 
    /// If the credentials are valid
    /// </summary>
    /// <returns>The result action</returns>
    public IActionResult Login(string username, string password)
    {
        // first make sure everything we got is valid
        if (string.IsNullOrWhiteSpace(password)) return BadRequest("No password given");
        if (string.IsNullOrWhiteSpace(username)) return BadRequest("No username given");

        // attempt to get the user from the database
        User? user = database.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            // if user does not exist return invalid credential message
            return Conflict("Invalid Credentials");
        }

        // see if the passwords match..... we need to remember to add the pepper here
        PasswordVerificationResult pwordMatch = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password + authOptions.PasswordPepper);

        // if pword is wrong return the invlalid message
        if (pwordMatch == PasswordVerificationResult.Failed)
        {
            return Conflict("Invalid Credentials");
        }

        // NOTE: WE NEED TO AT SOME POINT REHASH PASSWORDS WHEN NEEDED

        // If we made it here all is correct so we can procede with the login
        HttpContext.Session.SetString("Username", username);
        HttpContext.Session.SetInt32("UserID", user.Id);
        return Ok();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
