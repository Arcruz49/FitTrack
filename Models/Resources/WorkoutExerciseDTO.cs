namespace FitTrack.Models.Resources;

public class WorkoutExerciseDTO
{
    public int workoutId { get; set; }
    public string letter { get; set; }
    public string  workoutName{ get; set; }
    public string  description{ get; set; }
    public List<ExerciseDTO> exercises { get; set; } = new();
}