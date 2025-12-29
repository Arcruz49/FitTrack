using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitTrack.Models
{
    public class UserWorkouts
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public string? letter { get; set; } 
        public DateTime creation_date { get; set; }

        [ForeignKey("userId")]
        public Users User { get; set; }

    }
}
