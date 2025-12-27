using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitTrack.Services.Interfaces;
using FitTrack.Utils;
using FitTrack.Models.Resources;

namespace FitTrack.Controllers;

public class WorkoutController : BaseController
{
    
    public readonly IWorkoutService _workoutService;
    public readonly IExerciseService _exerciseService;
    public readonly Util _util;
    public WorkoutController(IWorkoutService workoutService, IExerciseService exerciseService, Util util)
    {
        _workoutService = workoutService;
        _exerciseService = exerciseService;
        _util = util;
    }

    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    [HttpGet]
    public JsonResult GetWorkouts()
    {
        var workoutsResult = _workoutService.GetWorkoutsByUserId(UserId);

        if(!workoutsResult.success) return Json(new {success = false, message = "Erro ao carregar treinos"});

        return Json(new {success = true, message = "", data = workoutsResult.data});
    }

    [Authorize]
    [HttpPost]
    public JsonResult CreateWorkout(string name = "")
    {
        if(string.IsNullOrEmpty(name)) return Json(new { success = false, message = "Nome inválido" });

        var workoutCreateResult = _workoutService.CreateWorkout(UserId, name);

        if(!workoutCreateResult.success) return Json(new { success = false, message = workoutCreateResult.message });

        return Json(new {success = true, message = "Treino criado com sucesso"});
    }

    [Authorize]
    [HttpPost]
    public JsonResult DeleteWorkout(int id = 0)
    {
        var workoutDeleteResult = _workoutService.DeleteWorkout(UserId, id);

        if(!workoutDeleteResult.success) return Json(new { success = false, message = workoutDeleteResult.message });

        return Json(new {success = true, message = "Treino excluído com sucesso"});
    }

    [Authorize]
    [HttpGet]
    public JsonResult GetWorkoutDetails(int id = 0)
    {
        var workoutResult = _workoutService.GetWorkoutById(UserId, id);

        if(!workoutResult.success) return Json(new {success = false, message = workoutResult.message});

        return Json(new {success = true, message = "", data = workoutResult.data});
    }

    [Authorize]
    [HttpPost]
    public JsonResult EditWorkout(WorkoutDTO workout)
    {
        var workoutResult = _workoutService.EditWorkout(UserId, workout);

        if(!workoutResult.success) return Json(new {success = false, message = workoutResult.message});

        return Json(new {success = true, message = "Treino editado com sucesso", data = workout});
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
    [HttpPost]
    public JsonResult GetExercisesByWorkoutId(int workoutId = 0)
    {
        if(workoutId == 0) return Json(new {success = false, message = "Treino inválido"});
        
        var getExercisesResult = _exerciseService.GetExercisesByWorkoutId(UserId, workoutId);
        
        if(!getExercisesResult.success) return Json(new {success = false, message = getExercisesResult.message});

        return Json(new {success = true, data = getExercisesResult.data});
    }
    
}
