using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitTrack.Data;
using FitTrack.Utils;
using FitTrack.Models.Resources;

namespace FitTrack.Controllers;

public class UserController : Controller
{
    
    private Context db;
    public Util util = new Util();
    public UserController(Context context)
    {
        db = context;
    }

    [Authorize]
    public IActionResult Index()
    {

        int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

        var user = db.Users.Where(a => a.id == userId).Select(a => new UsersDTO
        {
            id = a.id,
            name = a.name,
            email = a.email,
            bio = a.bio,
            phoneNumber = util.FormatPhoneNumber(a.phoneNumber ?? ""),
            creation_date = a.creation_date,
            creation_date_string = util.FormatCreationDate(a.creation_date)
        }).FirstOrDefault();

        if(user == null) return Json(new {success = false, message = "Usuário não encontrado"});


        return View(user);
    }
}