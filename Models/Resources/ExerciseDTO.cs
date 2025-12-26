namespace FitTrack.Models.Resources;

public class ExerciseDTO
{
    public int id {get; set;}
    public string name {get; set;} 
    public decimal? weight {get; set;} 
    public int? reps {get; set;} 
    public int? series {get; set;} 
    public int? rest {get; set;} 
    public string obs {get; set;} 
    public DateTime creation_date {get; set;} 
    public int? order {get; set;} 
}