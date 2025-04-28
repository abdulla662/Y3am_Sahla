using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sahla.Models
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }

        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }
    }
}
