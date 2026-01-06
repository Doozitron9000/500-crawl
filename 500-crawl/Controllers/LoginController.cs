using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _500_crawl.Models;
using _500_crawl.Authentication;
using Microsoft.EntityFrameworkCore;

namespace _500_crawl.Controllers;

public class LoginController : Controller
{
    // the database to which new users are sent and against which users are validated
    private readonly SiteDbContext database;

    public LoginController(SiteDbContext database)
    {   
        this.database = database;
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
        // create a new user.
        User user = new User
        {
            Username = username,
            PasswordHash = "123456"
        };
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
        return RedirectToAction("Index", "Login");
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
        // attempt to get the user from the database
        User? user = database.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            // user does not exist → back to login
            return RedirectToAction("Index", "Login");
        }
        // Start a new session
        HttpContext.Session.SetString("Username", username);
        return RedirectToAction("Index", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
