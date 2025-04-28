using System.ComponentModel.DataAnnotations;

namespace sahla.Models
{
    public class Badge
    {
        [Key]
        public int BadgeId { get; set; }

        public string BadgeName { get; set; }
        public int PointsRequired { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EarnedAt { get; set; }

        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
