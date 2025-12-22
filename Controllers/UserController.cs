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

    public JsonResult SaveProfileInfo([FromBody] UsersDTO ProfileInfo)
    {
        try
        {
            #region validacoes
            
            if(string.IsNullOrEmpty(ProfileInfo.name)) return Json(new {success = false, message = "Nome inválido", data = ProfileInfo});
            if(string.IsNullOrEmpty(ProfileInfo.email)) return Json(new {success = false, message = "Email inválido", data = ProfileInfo});
            
            #endregion

            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Json(new { success = false, message = "Usuário não autenticado" });

            int userId = int.Parse(userIdClaim);

            var user = db.Users.Where(a => a.id == userId).FirstOrDefault();
            var userMetrics = db.UserMetrics.Where(a => a.userId == userId).FirstOrDefault();

            if(user == null) return Json(new {success = false, message = "Usuário não encontrado"});
            

            // Users table
            user.name = ProfileInfo.name;
            user.email = ProfileInfo.email;
            user.bio = ProfileInfo.bio;
            user.phoneNumber = ProfileInfo.phoneNumber;
            user.phoneNumber = ProfileInfo.phoneNumber;

            // Metrics table --melhorar isso aqui depois
            if(userMetrics == null)
            {
                userMetrics = new UserMetrics()
                {
                    userId = userId,
                    weight = ProfileInfo.weight,
                    height = ProfileInfo.height,
                    bodyFat = ProfileInfo.bodyFat,
                    armCircumference = ProfileInfo.armCircumference,
                    chestCircumference = ProfileInfo.chestCircumference,
                    waistCircumference = ProfileInfo.waistCircumference,
                    legCircumference = ProfileInfo.legCircumference,
                    weightGoal = ProfileInfo.weightGoal,
                    workoutsGoal = ProfileInfo.workoutsGoal,
                };
                
                db.UserMetrics.Add(userMetrics);
            }
            else
            {
                userMetrics.weight = ProfileInfo.weight;
                userMetrics.height = ProfileInfo.height;
                userMetrics.bodyFat = ProfileInfo.bodyFat;
                userMetrics.armCircumference = ProfileInfo.armCircumference;
                userMetrics.chestCircumference = ProfileInfo.chestCircumference;
                userMetrics.waistCircumference = ProfileInfo.waistCircumference;
                userMetrics.legCircumference = ProfileInfo.legCircumference;
                userMetrics.weightGoal = ProfileInfo.weightGoal;
                userMetrics.workoutsGoal = ProfileInfo.workoutsGoal;
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Cadastro atualizado" });


        }
        catch(Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) , data = ProfileInfo});
        }
    }


    public async Task<JsonResult> UploadProfilePicture(IFormFile profilePic)
    {
        if (profilePic == null || profilePic.Length == 0)
            return Json(new { success = false, message = "Imagem inválida" });

        var userId = int.Parse(User.FindFirst("UserId")!.Value);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profilePic.FileName)}";
        var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await profilePic.CopyToAsync(stream);
        }

        var user = db.Users.First(x => x.id == userId);

        if (!string.IsNullOrWhiteSpace(user.profilePic))
        {
            var oldFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                user.profilePic.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
        }

        user.profilePic = "/uploads/" + fileName;

        db.SaveChanges();

        return Json(new
        {
            success = true,
            message = "Foto atualizada com sucesso",
            imageUrl = user.profilePic
        });
    }
}