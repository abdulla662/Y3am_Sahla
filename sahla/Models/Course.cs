using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations;

namespace sahla.Models
{
    public class Course
    {
        [Key]
        public int CoursId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public string TeacherId { get; set; } // ✅ New line


        // Navigation properties
        public ICollection<Lesson> Lessons { get; set; }
        public ICollection<Test> Tests { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}
