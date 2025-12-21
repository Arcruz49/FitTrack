using System;

namespace FitTrack.Models
{
    public class UserMetrics
    {
        public int id { get; set; }
        public int userId { get; set; }
        public decimal weight { get; set; }
        public decimal height { get; set; }
        public decimal bodyFat { get; set; }
        public decimal armCircumference { get; set; }
        public decimal chestCircumference { get; set; }
        public decimal waistCircumference { get; set; }
        public decimal legCircumference { get; set; }
        public decimal weightGoal { get; set; }
        public int workoutsGoal { get; set; }

        public Users? user { get; set; }

    }
}
