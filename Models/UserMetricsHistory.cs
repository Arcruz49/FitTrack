using System.ComponentModel.DataAnnotations.Schema;

namespace FitTrack.Models
{
    public class UserMetricsHistory
    {
        public int id { get; set; }
        public int userMetricsId { get; set; }
        public int userId { get; set; }
        public decimal? weight { get; set; }
        public decimal? height { get; set; }
        public decimal? bodyFat { get; set; }
        public decimal? armCircumference { get; set; }
        public decimal? chestCircumference { get; set; }
        public decimal? waistCircumference { get; set; }
        public decimal? legCircumference { get; set; }
        public decimal? weightGoal { get; set; }
        public decimal? workoutsGoal { get; set; }
        public DateTime creation_date { get; set; }

        [ForeignKey("userId")]
        public Users? user { get; set; }

        [ForeignKey("userMetricsId")]
        public UserMetrics? userMetrics { get; set; }

    }
}
