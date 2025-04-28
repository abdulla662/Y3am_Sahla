using sahla.DataAcess;
using sahla.Models;
using sahla.Repositories.Ireposotries;

namespace sahla.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private readonly ApplicationDbContext dbcontext;
        public CourseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbcontext = dbContext;
        }

    }
}

