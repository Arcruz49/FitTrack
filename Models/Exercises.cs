using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitTrack.Models
{
    public class Exercises
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string? name { get; set; }
        public decimal? weight { get; set; }
        public int? reps { get; set; }
        public int? series { get; set; }
        public int? rest { get; set; }
        public string? obs { get; set; }
        public int? order { get; set; }
        public int workoutId { get; set; }
        public DateTime creation_date { get; set; }

        [ForeignKey("userId")]
        public Users User { get; set; }

        [ForeignKey("workoutId")]
        public UserWorkout Workout { get; set; }

    }
}
