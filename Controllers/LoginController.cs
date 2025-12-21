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

    public LoginController(Context context)
    {
        db = context;
    }
    private readonly Context db;
    public Util util = new Util();
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public JsonResult GetUsers()
    {
        try
        {
            var users = db.Users.ToList();
            return Json(users);
        }
        catch(Exception ex)
        {
            return Json(new {success = false, message = ex.Message});
        }

    }

    [HttpPost]
    public JsonResult Login(string email = "", string password = "")
    {
        try
        {
            if (string.IsNullOrEmpty(email))
                return Json(new { success = false, message = "Usuário inválido" });

            if (string.IsNullOrEmpty(password))
                return Json(new { success = false, message = "Senha inválida" });

            var usuario = db.Users.FirstOrDefault(a => a.email == email);
            if (usuario == null)
                return Json(new { success = false, message = "Usuário não encontrado" });

            var passwordHasher = new PasswordHasher<Users>();
            var result = passwordHasher.VerifyHashedPassword(usuario, usuario.password, password);

            if (result != PasswordVerificationResult.Success)
                return Json(new { success = false, message = "Senha incorreta" });

            var profile = db.Profiles.Where(a => a.id == usuario.profileId).FirstOrDefault();

            // Criar claims do usuário
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.name),
                new Claim("UserId", usuario.id.ToString()),
                new Claim("Admin", (profile.admin ?? false).ToString()),
                new Claim("Email", usuario.email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
            };

            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return Json(new { success = true, message = "Login realizado com sucesso" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }

    [HttpPost]
    public async Task<JsonResult> Register(string name = "", string email = "", string password = "", string confirmPassword = "")
    {
        try
        {

            if (string.IsNullOrEmpty(name))
                return Json(new { success = false, message = "Nome inválido" });

            if (string.IsNullOrEmpty(email))
                return Json(new { success = false, message = "Email inválido" });

            if (string.IsNullOrEmpty(password))
                return Json(new { success = false, message = "Senha inválida" });

            if (!password.Equals(confirmPassword))
                return Json(new { success = false, message = "As senhas não coincidem" });

            var existingUser = db.Users.FirstOrDefault(a => a.email == email);
            if (existingUser != null)
                return Json(new { success = false, message = "Email já registrado" });

            // Cria o novo usuário
            var newUser = new Users
            {
                name = name,
                email = email,
                profileId = 2 
            };

            // Hashear a senha corretamente
            var passwordHasher = new PasswordHasher<Users>();
            newUser.password = passwordHasher.HashPassword(newUser, password);

            db.Users.Add(newUser);
            db.SaveChanges();

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
