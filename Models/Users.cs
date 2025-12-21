using System;

namespace FitTrack.Models
{
    public class Users
    {
        public int id { get; set; }
        public int profileId { get; set; }
        public string? email { get; set; }
        public string? name { get; set; }
        public string? password { get; set; }
        public string? bio { get; set; }
        public string? phoneNumber { get; set; }

        public DateTime creation_date { get; set; }

        public Profiles? profile { get; set; }

    }
}
