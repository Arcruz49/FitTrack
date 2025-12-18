using System;

namespace FitTrack.Models
{
    public class Profiles
    {
        public int id { get; set; }
        public string? type { get; set; }
        public bool? admin { get; set; }
        public DateTime creation_date { get; set; }

        public ICollection<Users>? users { get; set; }

    }
}
