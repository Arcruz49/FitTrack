using System;

namespace FitTrack.Models.Resources
{
    public class UsersDTO
    {
        public int id { get; set; }
        // public int profileId { get; set; }
        public string? email { get; set; }
        public string? name { get; set; }
        public string? bio { get; set; }
        public string? phoneNumber { get; set; }
        public string? profilePic { get; set; }
        public DateTime creation_date { get; set; }
        public string creation_date_string { get; set; }

        public decimal weight { get; set; }
        public decimal height { get; set; }
        public decimal bodyFat { get; set; }
        public decimal armCircumference { get; set; }
        public decimal chestCircumference { get; set; }
        public decimal waistCircumference { get; set; }
        public decimal legCircumference { get; set; }
        public decimal weightGoal { get; set; }
        public int workoutsGoal { get; set; }

    }
}
