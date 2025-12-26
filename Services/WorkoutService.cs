using FitTrack.Services.Interfaces;
using FitTrack.Data;
using FitTrack.Models;
using FitTrack.Models.Resources;
using FitTrack.Utils;


namespace FitTrack.Services;
public class WorkoutService : IWorkoutService
{

    private readonly Context _db;
    private readonly Util _util;

    public WorkoutService(Context db, Util util)
    {
        _db = db;
        _util = util;
    }
    
    
    public RetornoGenerico<List<WorkoutDTO>> GetWorkoutsByUserId(int userId)
    {
        var workouts = _db.UserWorkouts.Where(a => a.userId == userId).Select(a => new WorkoutDTO()
            {
                id = a.id,
                name = a.name,
                description = a.description,
                letter = a.letter
            }).ToList();

        return new RetornoGenerico<List<WorkoutDTO>>{success = true, data = workouts};
    }

    public Retorno CreateWorkout(int userId, string name)
    {
        var usedLetters = _db.UserWorkouts
        .Where(w => w.userId == userId)
        .Select(w => w.letter.ToUpper())
        .ToHashSet();

        string nextLetter = "";

        for (char c = 'A'; c <= 'Z'; c++)
        {
            if (!usedLetters.Contains(c.ToString()))
            {
                nextLetter = c.ToString();
                break;
            }
        }

        if (nextLetter == "")
            return new Retorno{success = true, message = "Limite máximo de treinos atingidos"};

        var workout = new UserWorkout()
        {
            userId = userId,
            name = name,
            // description = newWorkout.description,
            description = "",
            letter = nextLetter,
        };

        try
        {
            _db.UserWorkouts.Add(workout);
            _db.SaveChanges();
        }
        catch(Exception ex)
        {
            return new Retorno{success = true, message = _util.ErrorMessage(ex)};
        }
        
        return new Retorno{success = true};
    }

    public Retorno DeleteWorkout(int userId, int id)
    {
        var workout = _db.UserWorkouts.Where(a => a.id == id && a.userId == userId).FirstOrDefault();

        if(workout == null) return new Retorno{success = false, message = "Treino não encontrado"};

        try
        {
            _db.UserWorkouts.Remove(workout);
            _db.SaveChanges();
        }
        catch(Exception ex)
        {
            return new Retorno{success = true, message = _util.ErrorMessage(ex)};
        }

        return new Retorno{success = true};
    }

    public RetornoGenerico<WorkoutDTO> GetWorkoutById(int userId, int id)
    {
        var workout = _db.UserWorkouts.Where(a => a.id == id && a.userId == userId).Select(a => new WorkoutDTO
        {
            id = a.id,
            name = a.name,
            description = a.description,
            letter = a.letter
        }).FirstOrDefault();

        if(workout == null) return new RetornoGenerico<WorkoutDTO>{ success = false, message = "Treino não encontrado" };

        return new RetornoGenerico<WorkoutDTO>{success = true, data = workout};
    }

    public Retorno EditWorkout(int userId, WorkoutDTO workout)
    {

        var workoutOld = _db.UserWorkouts.Where(a => a.id == workout.id && a.userId == userId).FirstOrDefault();

        if(workoutOld == null) return new Retorno{success = false, message = "Treino não encontrado"};

        workoutOld.name = workout.name;
        workoutOld.description = workout.description;

        try
        {
            _db.SaveChanges();
        }
        catch(Exception ex)
        {
            return new Retorno{success = true, message = _util.ErrorMessage(ex)};
        }

        return new Retorno{success = true};
    }


}