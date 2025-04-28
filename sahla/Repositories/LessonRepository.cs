using sahla.DataAcess;
using sahla.Models;
using sahla.Repositories.Ireposotries;

namespace sahla.Repositories
{
    public class LessonRepository : Repository<Lesson>, ILessonRepository
    {
        private readonly ApplicationDbContext dbcontext;
        public LessonRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbcontext = dbContext;
        }

    }
}
