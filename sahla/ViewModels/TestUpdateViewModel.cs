public class TestUpdateViewModel
{
    public int TestId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Partition { get; set; }
    public int CourseId { get; set; }

    public List<UpdateQuestionViewModel> Questions { get; set; }
}

public class UpdateQuestionViewModel
{
    public int? QuestionId { get; set; }
    public string QuestionText { get; set; }
    public int Points { get; set; }
    public string QuestionType { get; set; }

    public List<UpdateAnswerViewModel> Answers { get; set; }
}

public class UpdateAnswerViewModel
{
    public int? AnswerId { get; set; }
    public string AnswerText { get; set; }
    public bool IsCorrect { get; set; }
}
