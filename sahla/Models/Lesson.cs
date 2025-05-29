using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sahla.Models
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        public string Title { get; set; }
        public string ContentType { get; set; }
        public string ContentUrl { get; set; }
        public string Section { get; set; } 

        public int LessonOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign key
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}
