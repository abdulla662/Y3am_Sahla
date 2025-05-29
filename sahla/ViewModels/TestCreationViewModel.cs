namespace sahla.ViewModel
{
        public class TestCreationViewModel
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Partition { get; set; }
        public int CourseId { get; set; }  

        public List<QuestionViewModel> Questions { get; set; }
        }

        public class QuestionViewModel
        {
            public string QuestionText { get; set; }
            public int Points { get; set; } = 1;
            public string QuestionType { get; set; }

            public List<AnswerViewModel> Answers { get; set; } = new();
        }

        public class AnswerViewModel
        {
            public string AnswerText { get; set; }
            public bool IsCorrect { get; set; }
        }
    }

