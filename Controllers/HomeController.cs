using Microsoft.AspNetCore.Mvc;
using FitTrack.Models;
using Microsoft.AspNetCore.Authorization;
using FitTrack.Utils;
using System.Security.Claims;
using FitTrack.Models.Resources;
using FitTrack.Services.Interfaces;

namespace FitTrack.Controllers;

public class HomeController : BaseController
{
    
    private readonly IExerciseService _exerciseService;
    private readonly Util _util;

    public HomeController(IExerciseService homeService, Util util)
    {
        _exerciseService = homeService;
        _util = util;
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
     int order = 0, int workoutId = 0)
    {
        #region validacoes

        if(name == "") return Json(new {success = false, message = "Nome inválido"});
        if(weight == 0) return Json(new {success = false, message = "Nome inválido"});
        if(workoutId == 0) return Json(new {success = false, message = "Treino inválido"});

        #endregion

        var createExerciseResult = _exerciseService.CreateExercise(UserId, name, weight, reps, series, rest, obs, order, workoutId);

        if(!createExerciseResult.success) return Json(new {success = false, message = createExerciseResult.message});
        
        return Json(new {success = true, message = "Exercício criado com sucesso.", data = createExerciseResult.data});
    }

    [Authorize]
    [HttpGet]
    public JsonResult GetExercises()
    {
        var exercisesResult = _exerciseService.GetExercisesByUserId(UserId);

        if(!exercisesResult.success) return Json(new {success = false, message = exercisesResult.message});

        return Json(new {success = true, message = "", data = exercisesResult.data});
    }

    [Authorize]
    [HttpGet]
    public JsonResult GetUserEmail()
    {
        return Json(new {success = true, data = User.FindFirst("Email")?.Value ?? ""});  
    }

    [Authorize]
    [HttpGet]
    public JsonResult GetExercicioById(int id = 0)
    {
        var exercicioResult = _exerciseService.GetExercicioById(UserId, id);

        if(!exercicioResult.success) return Json(new {success = false, message = "Exercício não encontrado", data = ""});

        return Json(new {success = true, data = exercicioResult.data});
    }

    [Authorize]
    [HttpPost]
    public JsonResult DeleteExerciseById(int id = 0)
    {
        var deleteExerciseResult = _exerciseService.DeleteExerciseById(UserId, id);
        
        if(!deleteExerciseResult.success) return Json(new {success = false, message = deleteExerciseResult.message});

        return Json(new {success = true, message = "Exercício excluído com sucesso"});
    }

    [Authorize]
    [HttpPost]
    public JsonResult EditExerciseById([FromBody] Exercises exerciseNew)
    {
        #region  validacoes

        if(exerciseNew.name == null || exerciseNew.name == "undefined" || exerciseNew.name == "")
            return Json(new {success = false, message = "Nome inválido"});

        if(exerciseNew.id == 0)
            return Json(new {success = false, message = "Exercício inválido"});

        #endregion

        var exercicioResult = _exerciseService.EditExerciseById(UserId, exerciseNew);
        
        if(!exercicioResult.success) return Json(new {success = false, message = exercicioResult.message});

        return Json(new {success = true, message = "Exercício atualizado com sucesso"});
    }  


    [Authorize]
    [HttpPost]
    public JsonResult UpdateExerciseOrder([FromBody] List<ExerciseOrder> exercises)
    {
        if (exercises == null || exercises.Count == 0)
            return Json(new { success = false, message = "Lista vazia" });

        var exerciseOrderResult = _exerciseService.UpdateExerciseOrder(UserId, exercises);

        if(!exerciseOrderResult.success) return Json(new { success = false, message = exerciseOrderResult.message});

        return Json(new { success = true, message = "Ordem atualizada com sucesso" });
    }

}
