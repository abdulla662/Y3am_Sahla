using System.ComponentModel.DataAnnotations;

namespace sahla.Models
{
    public class Challenge
    {
        [Key]
        public int ChallengeId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int PointsReward { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }

}
