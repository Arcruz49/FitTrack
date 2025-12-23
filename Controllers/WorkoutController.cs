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

        var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return Json(new { success = false, message = "Usuário não autenticado" });

        int userId = int.Parse(userIdClaim);

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


        return View(user);
    }

    [Authorize]
    [HttpGet]
    public JsonResult GetWorkouts()
    {
        try
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return Json(new { success = false, message = "Usuário não autenticado" });

            int userId = int.Parse(userIdClaim);

            var workouts = db.UserWorkouts.Where(a => a.userId == userId).Select(a => new WorkoutDTO()
            {
                id = a.id,
                name = a.name,
                description = a.description,
                letter = a.letter
            }).ToList();

            return Json(new {success = true, message = "", data = workouts});
        }
        catch(Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex)});
        }
    }

    [Authorize]
    [HttpPost]
    public JsonResult CreateWorkout(string name = "")
    {
        try
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return Json(new { success = false, message = "Usuário não autenticado" });

            int userId = int.Parse(userIdClaim);

             var usedLetters = db.UserWorkouts
            .Where(w => w.userId == userId)
            .Select(w => w.letter.ToUpper())
            .ToHashSet();

            string nextLetter = null;

            for (char c = 'A'; c <= 'Z'; c++)
            {
                if (!usedLetters.Contains(c.ToString()))
                {
                    nextLetter = c.ToString();
                    break;
                }
            }

            if (nextLetter == null)
                return Json(new { success = false, message = "Limite máximo de treinos atingido (A–Z)" });


            var workout = new UserWorkout()
            {
                userId = userId,
                name = name,
                // description = newWorkout.description,
                description = "",
                letter = nextLetter,
            };

            db.UserWorkouts.Add(workout);
            db.SaveChanges();


            return Json(new {success = true, message = "Treino criado com sucesso"});
        }
        catch(Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex)});
        }
    }


}