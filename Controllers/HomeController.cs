using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FitTrack.Models;
using Microsoft.AspNetCore.Authorization;
using FitTrack.Data;
using FitTrack.Utils;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using FitTrack.Models.Resources;

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
            }).OrderBy(a => a.order).ToList();

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
    public JsonResult EditExerciseById([FromBody] Exercises exerciseNew)
    {
        try
        {

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Json(new { success = false, message = "Usuário não autenticado" });

            int userId = int.Parse(userIdClaim.Value);

            #region  validacoes

            if(exerciseNew.name == null || exerciseNew.name == "undefined" || exerciseNew.name == "")
                return Json(new {success = false, message = "Nome inválido"});

            if(exerciseNew.id == 0)
                return Json(new {success = false, message = "Exercício inválido"});

            #endregion


            var exercicioOld = db.Exercises
                .Where(a => a.id == exerciseNew.id && a.userId == userId)
                .FirstOrDefault();

            if(exercicioOld == null) return Json(new {success = false, message = "Exercício não encontrado"});
            
            exercicioOld.name = exerciseNew.name;
            exercicioOld.weight = exerciseNew.weight;
            exercicioOld.reps = exerciseNew.reps;
            exercicioOld.series = exerciseNew.series;
            exercicioOld.obs = exerciseNew.obs;
            exercicioOld.rest = exerciseNew.rest;

            db.SaveChanges();
            
            return Json(new {success = true, message = "Exercício atualizado com sucesso"});


        }
        catch(Exception ex)
        {
            return Json(new {success = false, message = util.ErrorMessage(ex), data = ""});
        }

    }  


    [Authorize]
    [HttpPost]
    public JsonResult UpdateExerciseOrder([FromBody] List<ExerciseOrder> exercises)
    {
        try
        {
            if (exercises == null || exercises.Count == 0)
                return Json(new { success = false, message = "Lista vazia" });

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Json(new { success = false, message = "Usuário não autenticado" });

            int userId = int.Parse(userIdClaim.Value);

            var ids = exercises.Select(e => e.id).ToList();

            var exerciciosDb = db.Exercises
                .Where(e => ids.Contains(e.id) && e.userId == userId)
                .ToList();

            if (exerciciosDb.Count != exercises.Count)
                return Json(new { success = false, message = "Exercício inválido ou sem permissão" });

            foreach (var exercicio in exerciciosDb)
            {
                var newOrder = exercises.First(e => e.id == exercicio.id).order;
                exercicio.order = newOrder;
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Ordem atualizada com sucesso" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }

}
