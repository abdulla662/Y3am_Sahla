using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sahla.Models
{
    public class Test
    {
        [Key]
        public int TestId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }=DateTime.Now.ToLocalTime();
        public string Partition { get; set; } 

        // Foreign key
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        // Navigation property
        public ICollection<Question> Questions { get; set; }
    }
}
