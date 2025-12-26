using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitTrack.Services.Interfaces;
using FitTrack.Utils;
using FitTrack.Models.Resources;

namespace FitTrack.Controllers;

public class WorkoutController : BaseController
{
    
    public readonly IWorkoutService _workoutServices;
    public readonly Util _util;
    public WorkoutController(IWorkoutService workoutServices, Util util)
    {
        _workoutServices = workoutServices;
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
        var workoutsResult = _workoutServices.GetWorkoutsByUserId(UserId);

        if(!workoutsResult.success) return Json(new {success = false, message = "Erro ao carregar treinos"});

        return Json(new {success = true, message = "", data = workoutsResult.data});
    }

    [Authorize]
    [HttpPost]
    public JsonResult CreateWorkout(string name = "")
    {
        if(string.IsNullOrEmpty(name)) return Json(new { success = false, message = "Nome inválido" });

        var workoutCreateResult = _workoutServices.CreateWorkout(UserId, name);

        if(!workoutCreateResult.success) return Json(new { success = false, message = workoutCreateResult.message });

        return Json(new {success = true, message = "Treino criado com sucesso"});
    }

    [Authorize]
    [HttpPost]
    public JsonResult DeleteWorkout(int id = 0)
    {
        var workoutDeleteResult = _workoutServices.DeleteWorkout(UserId, id);

        if(!workoutDeleteResult.success) return Json(new { success = false, message = workoutDeleteResult.message });

        return Json(new {success = true, message = "Treino excluído com sucesso"});
    }

    [Authorize]
    [HttpGet]
    public JsonResult GetWorkoutDetails(int id = 0)
    {
        var workoutResult = _workoutServices.GetWorkoutById(UserId, id);

        if(!workoutResult.success) return Json(new {success = false, message = workoutResult.message});

        return Json(new {success = true, message = "", data = workoutResult.data});
    }

    [Authorize]
    [HttpPost]
    public JsonResult EditWorkout(WorkoutDTO workout)
    {
        var workoutResult = _workoutServices.EditWorkout(UserId, workout);

        if(!workoutResult.success) return Json(new {success = false, message = workoutResult.message});

        return Json(new {success = true, message = "Treino editado com sucesso", data = workout});
    }

}
