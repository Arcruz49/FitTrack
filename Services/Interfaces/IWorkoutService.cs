using FitTrack.Models;
using FitTrack.Models.Resources;

namespace FitTrack.Services.Interfaces;
public interface IWorkoutService
{
    RetornoGenerico<List<WorkoutDTO>> GetWorkoutsByUserId(int userId);
    Retorno CreateWorkout(int userId, string name);
    Retorno DeleteWorkout(int userId, int id);
    RetornoGenerico<WorkoutDTO> GetWorkoutById(int userId, int id);
}