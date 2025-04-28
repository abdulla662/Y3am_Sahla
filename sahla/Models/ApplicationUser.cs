using Microsoft.AspNetCore.Identity;

namespace sahla.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? ProfilePicture { get; set; }

        public string? Adress { get; set; }

        public int Points { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Progress> Progresses { get; set; }
        public ICollection<Badge> Badges { get; set; }
        public ICollection<Challenge> Challenges { get; set; }
    }
}
