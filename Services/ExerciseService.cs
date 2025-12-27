using FitTrack.Data;
using FitTrack.Models;
using FitTrack.Models.Resources;
using FitTrack.Utils;
using FitTrack.Services.Interfaces;

namespace FitTrack.Services;
public class ExerciseService : IExerciseService
{
    private readonly Context _db;
    private readonly Util _util;
    public ExerciseService(Context db, Util util)
    {
        _db = db;
        _util = util;
    }

    public RetornoGenerico<Exercises> CreateExercise(int userId, string name, decimal weight, int reps, int series, int rest, string obs,
     int order, int workoutId)
    {

        bool userExists = _db.Users.Any(u => u.id == userId);
        if(!userExists) return new RetornoGenerico<Exercises>{success = false, message = "Usuário não encontrado"}; 

        bool workoutExists = _db.UserWorkouts.Any(u => u.id == workoutId);
        if(!userExists) return new RetornoGenerico<Exercises>{success = false, message = "Treino não encontrado"}; 

        var exercise = new Exercises()
            {
                name = name,
                userId = userId,
                weight = weight,
                reps = reps,
                series = series,
                rest = rest,
                obs = obs,
                workoutId = workoutId,
            };

        try
        {
            _db.Exercises.Add(exercise);
            _db.SaveChanges();

            return new RetornoGenerico<Exercises>{success = true, data = exercise};
        }
        catch(Exception ex)
        {
            return new RetornoGenerico<Exercises>{success = false, message = _util.ErrorMessage(ex)};
        }
        
    }

    public RetornoGenerico<List<ExerciseDTO>> GetExercisesByUserId(int userId)
    {
        
        var exercicios = _db.Exercises.Where(a => a.userId == userId)
            .Select(a => new ExerciseDTO
            {
                id = a.id,
                name = a.name,
                weight = a.weight,
                reps = a.reps,
                series = a.series,
                creation_date = a.creation_date,
                order = a.order
            }).OrderBy(a => a.order).ToList();
        
        return new RetornoGenerico<List<ExerciseDTO>>{success = true, data = exercicios};
    }

    public RetornoGenerico<ExerciseDTO> GetExercicioById(int userId, int id)
    {
        var exercicio = _db.Exercises
                .Where(a => a.id == id && a.userId == userId)
                .Select(a => new ExerciseDTO
                {
                    id = a.id,
                    name = a.name,
                    weight = a.weight,
                    reps = a.reps,
                    series = a.series,
                    rest = a.rest,
                    obs = a.obs,
                    creation_date = a.creation_date,
                    order = a.order
                })
                .FirstOrDefault();
        
        if(exercicio == null) return new RetornoGenerico<ExerciseDTO>{success = false, message = "Exercício não encontrado"};

        return new RetornoGenerico<ExerciseDTO>{success = true, data = exercicio};
    }

    public Retorno DeleteExerciseById(int userId, int id)
    {
        var exercicio = _db.Exercises
                .Where(a => a.id == id && a.userId == userId)
                .FirstOrDefault();

        if(exercicio == null) return new Retorno{success = false, message = "Exercício não encontrado"};

        try
        {
            _db.Exercises.Remove(exercicio);
            _db.SaveChanges();
            return new Retorno{success = true, message = ""};
        }
        catch(Exception ex)
        {
            return new Retorno{success = false, message = _util.ErrorMessage(ex)};
        }
    }

     
    public Retorno EditExerciseById(int userId, Exercises exerciseNew)
    {
        var exercicioOld = _db.Exercises
                .Where(a => a.id == exerciseNew.id && a.userId == userId)
                .FirstOrDefault();

            if(exercicioOld == null) return new Retorno{success = false, message = "Exercício não encontrado"};
            
            exercicioOld.name = exerciseNew.name;
            exercicioOld.weight = exerciseNew.weight;
            exercicioOld.reps = exerciseNew.reps;
            exercicioOld.series = exerciseNew.series;
            exercicioOld.obs = exerciseNew.obs;
            exercicioOld.rest = exerciseNew.rest;

        try
        {
            _db.SaveChanges();
            return new Retorno{success = true, message = ""};
        }
        catch(Exception ex)
        {
            return new Retorno{success = false, message = _util.ErrorMessage(ex)};
        }
            
    }

    public Retorno UpdateExerciseOrder(int userId, List<ExerciseOrder> exercises)
    {
        var ids = exercises.Select(e => e.id).ToList();

        var exerciciosDb = _db.Exercises
            .Where(e => ids.Contains(e.id) && e.userId == userId)
            .ToList();

        if (exerciciosDb.Count != exercises.Count)
            return new Retorno{success = false, message = "Exercício inválido ou sem permissão"};

        foreach (var exercicio in exerciciosDb)
        {
            var newOrder = exercises.First(e => e.id == exercicio.id).order;
            exercicio.order = newOrder;
        }

        try
        {
            _db.SaveChanges();
            return new Retorno{success = true, message = ""};
        }
        catch(Exception ex)
        {
            return new Retorno{success = false, message = _util.ErrorMessage(ex)};
        }
    }

    public RetornoGenerico<List<ExerciseDTO>> GetExercisesByWorkoutId(int userId, int workoutId)
    {
        var exercicios = _db.Exercises.Where(a => a.userId == userId && a.workoutId == workoutId)
            .Select(a => new ExerciseDTO
            {
                id = a.id,
                name = a.name,
                weight = a.weight,
                reps = a.reps,
                series = a.series,
                creation_date = a.creation_date,
                order = a.order
            }).OrderBy(a => a.order).ToList();
        
        return new RetornoGenerico<List<ExerciseDTO>>{success = true, data = exercicios};
    }



    
}
