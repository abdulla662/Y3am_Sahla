using sahla.DataAcess;
using sahla.Repositories.Ireposotries;

namespace sahla.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Answers = new AnswerRepository(_context);
            ApplicationUsers = new ApplicationUserRepository(_context);
            Badges = new BadgeRepository(_context);
            Challenges = new ChallengeRepository(_context);
            Courses = new CourseRepository(_context);
            Lessons = new LessonRepository(_context);
            Progresses = new ProgressRepository(_context);
            Questions = new QuestionRepository(_context);
            Tests = new TestRepository(_context);
        }

        public IAnswerRepository Answers { get; private set; }
        public IApplicationUserRepository ApplicationUsers { get; private set; }
        public IBadgeRepository Badges { get; private set; }
        public IChallengeRepository Challenges { get; private set; }
        public ICourseRepository Courses { get; private set; }
        public ILessonRepository Lessons { get; private set; }
        public IProgressRepository Progresses { get; private set; }
        public IQuestionRepository Questions { get; private set; }
        public ITestRepository Tests { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
