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
        string userEmail = User.FindFirst("Email")?.Value;
        int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

        // var countExercise = db.Exercises.Where(a => a.userId == userId); 
        // ViewBag.countExercise = userName;

        ViewBag.userName = userName;
        ViewBag.userEmail = userEmail;

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

            return Json(new {success = true, message = "Exercício criado com sucesso.", data = exercise});

        }
        catch(Exception ex)
        {
            return Json(new {success = false, message = util.ErrorMessage(ex)});
        }
    }

    [Authorize]
    [HttpGet]
    public JsonResult GetExercises()
    {
        try
        {
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            
            var exercicios = db.Exercises.Where(a => a.userId == userId)
            .Select(a => new
            {
                a.id,
                a.name,
                a.weight,
                a.reps,
                a.series,
                a.creation_date,
                a.order
            }).ToList();

            return Json(new {success = true, message = "", data = exercicios});
        }
        catch(Exception ex)
        {
            return Json(new {success = false, message = util.ErrorMessage(ex)});
        }
    }

    [Authorize]
    [HttpGet]
    public JsonResult GetUserEmail()
    {
        try
        {
            return Json(new {success = true, data = User.FindFirst("Email")?.Value ?? ""});
        }
        catch(Exception ex)
        {
            return Json(new {success = false, message = util.ErrorMessage(ex), data = ""});
        }

    }

    [Authorize]
    [HttpGet]
    public JsonResult GetExercicioById(int id = 0)
    {
        try
        {
            if(id == 0) return Json(new {success = false, message = ""});

            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

            var exercicio = db.Exercises
                .Where(a => a.id == id && a.userId == userId)
                .FirstOrDefault();

            if(exercicio == null) return Json(new {success = false, message = "Exercício não encontrado", data = ""});

            return Json(new {success = true, data = exercicio});
        }
        catch(Exception ex)
        {
            return Json(new {success = false, message = util.ErrorMessage(ex), data = ""});
        }

    }

    [Authorize]
    [HttpPost]
    public JsonResult DeleteExerciseById(int id = 0)
    {
        try
        {
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            var exercicio = db.Exercises
                .Where(a => a.id == id && a.userId == userId)
                .FirstOrDefault();

            if(exercicio == null) return Json(new {success = false, message = "Exercício não encontrado", data = ""});
            
            db.Exercises.Remove(exercicio);
            db.SaveChanges();

            return Json(new {success = true, message = "Exercício excluído com sucesso", data = ""});

        }
        catch(Exception ex)
        {
            return Json(new {success = false, message = util.ErrorMessage(ex), data = ""});
        }
    }

    [Authorize]
    [HttpPost]
    public JsonResult EditExerciseById(int id = 0)
    {
        try
          {

	     int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
             var exercicio = db.Exercises
                .Where(a => a.id == id && a.userId == userId)
                .FirstOrDefault();

             if(exercicio == null) return Json(new {success = false, message = "Exercício não enco>
          return Json(new {success = true, message = "Exercício excluído com sucesso"});


	  }
          catch(Exception ex)
	  {
	     return Json(new {success = false, message = util.ErrorMessage(ex), data = ""});
	  }

    }  
}
