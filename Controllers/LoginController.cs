using Microsoft.AspNetCore.Mvc;
using FitTrack.Models;
using FitTrack.Utils;
using FitTrack.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
namespace FitTrack.Controllers;

public class LoginController : Controller
{
    private readonly IAuthService _authService;
    public Util util = new Util();


    public LoginController(IAuthService authService)
    {
        _authService = authService;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<JsonResult> Login(string email = "", string password = "")
    {
    
        var authResult = _authService.Authenticate(email, password);

        if (!authResult.success)
            return Json(authResult);

        var profileResult = _authService.GetProfile(authResult.data!.profileId);

        var user = authResult.data!;
        var profile = profileResult.data!;


        if (!profileResult.success)
            return Json(profileResult);
            
        // Criar claims do usuário
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.name),
            new Claim("UserId", user.id.ToString()),
            new Claim("Admin", (profile.admin ?? false).ToString()),
            new Claim("Email", user.email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        return Json(new { success = true, message = "Login realizado com sucesso" });
    }

    [HttpPost]
    public async Task<JsonResult> Register(string name = "", string email = "", string password = "", string confirmPassword = "")
    {
        try
        {

            var registerResult = _authService.Register(name, email, password, confirmPassword);

            if (!registerResult.success)
            {
                
            }
            // Criar claims do usuário
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.name ?? ""),
                new Claim("UserId", newUser.id.ToString()),
                new Claim("Admin", "False") // Usuário normal
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return Json(new { success = true, message = "Registro realizado com sucesso" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Login");
    }

    
}
