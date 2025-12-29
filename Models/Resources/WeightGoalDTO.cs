namespace FitTrack.Models.Resources;

public class weightGoalDTO
{
    public decimal weight { get; set; }
    public decimal weightGoal { get; set; }
    public decimal difference { get; set; }
    public bool loseWeigth { get; set; }
    public bool completed { get; set; }
}