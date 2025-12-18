using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FitTrack.Models;
using Microsoft.AspNetCore.Authorization;
using FitTrack.Data;
using FitTrack.Utils;
using System.Security.Claims;

namespace FitTrack.Controllers;

public class HomeController : Controller
{
    
    private Context db;
    public Util util = new Util();
    public HomeController(Context context)
    {
        db = context;
    }

    [Authorize]
    public IActionResult Index()
    {
        string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        ViewBag.userName = userName;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
