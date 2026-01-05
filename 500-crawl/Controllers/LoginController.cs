using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _500_crawl.Models;

namespace _500_crawl.Controllers;

public class LoginController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
