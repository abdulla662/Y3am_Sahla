namespace sahla.DTOs.Response
{
    public class TestDetailsDto
    {
        public int TestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Partition { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<QuestionDto> Questions { get; set; }
    }

    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int Points { get; set; }
        public string QuestionType { get; set; }

        public List<AnswerDto> Answers { get; set; }
    }

    public class AnswerDto
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
