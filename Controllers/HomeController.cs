using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FitTrack.Models;
using Microsoft.AspNetCore.Authorization;
using FitTrack.Data;
using FitTrack.Utils;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

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

    [Authorize]
    [HttpPost]
    public JsonResult CreateExercise(string name = "", decimal weight = 0, int reps = 0, int series = 0, int rest = 0, string obs = "",
     int order = 0)
    {
        try
        {
            #region validacoes

            if(name == "") return Json(new {success = false, message = "Nome inválido"});
            if(weight == 0) return Json(new {success = false, message = "Nome inválido"});
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            if(userId == 0) return Json(new {success = false, message = "Usuário inválido, faça login novamente"});

            #endregion


            var exercise = new Exercises()
            {
                name = name,
                userId = userId,
                weight = weight,
                reps = reps,
                series = series,
                rest = rest,
                obs = obs,
            };

            db.Exercises.Add(exercise);
            db.SaveChanges();

            return Json(new {success = true, message = "Exercício criado com sucesso."});

        }
        catch(Exception ex)
        {
            return Json(new {success = false, message = util.ErrorMessage(ex)});
        }
    }

  
}
