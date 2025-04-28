using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sahla.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        public string QuestionText { get; set; }
        public int Points { get; set; }
        public string QuestionType { get; set; }

        // Foreign key
        public int TestId { get; set; }
        [ForeignKey("TestId")]
        public Test Test { get; set; }

        // Navigation property
        public ICollection<Answer> Answers { get; set; }
    }
}
