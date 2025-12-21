using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitTrack.Data;
using FitTrack.Utils;
using FitTrack.Models.Resources;
using FitTrack.Models;

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

    public JsonResult SaveProfileInfo([FromBody] UsersDTO userNewInfo)
    {
        try
        {
            #region validacoes
            
            if(string.IsNullOrEmpty(userNewInfo.name)) return Json(new {success = false, message = "Nome inválido"});
            if(string.IsNullOrEmpty(userNewInfo.email)) return Json(new {success = false, message = "Email inválido"});
            
            #endregion

            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

            var user = db.Users.Where(a => a.id == userId).FirstOrDefault();
            var userMetrics = db.UserMetrics.Where(a => a.userId == userId).FirstOrDefault();

            if(user == null) return Json(new {success = false, message = "Usuário não encontrado"});
            

            // Users table
            user.name = userNewInfo.name;
            user.email = userNewInfo.email;
            user.bio = userNewInfo.bio;
            user.phoneNumber = userNewInfo.phoneNumber;
            user.phoneNumber = userNewInfo.phoneNumber;

            // Metrics table --melhorar isso aqui depois
            if(userMetrics == null)
            {
                userMetrics = new UserMetrics()
                {
                    weight = userNewInfo.weight,
                    height = userNewInfo.height,
                    bodyFat = userNewInfo.bodyFat,
                    armCircumference = userNewInfo.armCircumference,
                    chestCircumference = userNewInfo.chestCircumference,
                    waistCircumference = userNewInfo.waistCircumference,
                    legCircumference = userNewInfo.legCircumference,
                    weightGoal = userNewInfo.weightGoal,
                    workoutsGoal = userNewInfo.workoutsGoal,
                };
                
                db.UserMetrics.Add(userMetrics);
            }
            else
            {
                userMetrics.weight = userNewInfo.weight;
                userMetrics.height = userNewInfo.height;
                userMetrics.bodyFat = userNewInfo.bodyFat;
                userMetrics.armCircumference = userNewInfo.armCircumference;
                userMetrics.chestCircumference = userNewInfo.chestCircumference;
                userMetrics.waistCircumference = userNewInfo.waistCircumference;
                userMetrics.legCircumference = userNewInfo.legCircumference;
                userMetrics.weightGoal = userNewInfo.weightGoal;
                userMetrics.workoutsGoal = userNewInfo.workoutsGoal;
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Cadastro atualizado" });


        }
        catch(Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }
}