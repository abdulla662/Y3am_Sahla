using System.ComponentModel.DataAnnotations;

namespace sahla.Models
{
    public class Progress
    {
        [Key]
        public int ProgressId { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
