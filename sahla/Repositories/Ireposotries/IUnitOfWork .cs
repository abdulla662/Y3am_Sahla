namespace sahla.Repositories.Ireposotries
{
    public interface IUnitOfWork
    {
        IAnswerRepository Answers { get; }
        IApplicationUserRepository ApplicationUsers { get; }
        IBadgeRepository Badges { get; }
        IChallengeRepository Challenges { get; }
        ICourseRepository Courses { get; }
        ILessonRepository Lessons { get; }
        IProgressRepository Progresses { get; }
        IQuestionRepository Questions { get; }
        ITestRepository Tests { get; }

        Task<int> CompleteAsync();


    }
}
