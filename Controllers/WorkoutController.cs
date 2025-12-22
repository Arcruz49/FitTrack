using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitTrack.Data;
using FitTrack.Utils;
using FitTrack.Models.Resources;
using FitTrack.Models;

namespace FitTrack.Controllers;

public class WorkoutController : Controller
{
    
    private Context db;
    public Util util = new Util();
    public WorkoutController(Context context)
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
            profilePic = string.IsNullOrWhiteSpace(a.profilePic) ? Url.Content("~/images/default-pf.png"): Url.Content(a.profilePic),
            creation_date = a.creation_date,
            creation_date_string = util.FormatCreationDate(a.creation_date),
        }).FirstOrDefault();

        if(user == null) return Json(new {success = false, message = "Usuário não encontrado"});


        var metrics = db.UserMetrics.Where(a => a.userId == userId).FirstOrDefault();

        if(metrics != null)
        {
            user.weight = metrics.weight;
            user.height = metrics.height;
            user.bodyFat = metrics.bodyFat;
            user.armCircumference = metrics.armCircumference;
            user.chestCircumference = metrics.chestCircumference;
            user.waistCircumference = metrics.waistCircumference;
            user.legCircumference = metrics.legCircumference;
            user.weightGoal = metrics.weightGoal;
        }



        return View(user);
    }


}