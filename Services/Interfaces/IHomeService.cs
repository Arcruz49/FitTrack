using FitTrack.Models;
using FitTrack.Models.Resources;

namespace FitTrack.Services.Interfaces;
public interface IHomeService
{
    RetornoGenerico<Exercises> CreateExercise(int userId, string name, decimal weight, int reps, int series, int rest, string obs,
     int order);
    RetornoGenerico<List<ExerciseDTO>> GetExercisesByUserId(int userId);
    RetornoGenerico<ExerciseDTO> GetExercicioById(int userId, int id);
    Retorno DeleteExerciseById(int userId, int id);
    Retorno EditExerciseById(int userId, Exercises exerciseNew);
    Retorno UpdateExerciseOrder(int userId, List<ExerciseOrder> exercises);
}
