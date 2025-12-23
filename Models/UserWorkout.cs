using System;

namespace FitTrack.Models
{
    public class UserWorkout
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public string? letter { get; set; } 
        public DateTime creation_date { get; set; }

        public Users? user { get; set; }

    }
}
