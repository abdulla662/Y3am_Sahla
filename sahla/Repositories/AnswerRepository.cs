using sahla.DataAcess;
using sahla.Models;
using sahla.Repositories.Ireposotries;

namespace sahla.Repositories
{
    public class AnswerRepository : Repository<Answer>, IAnswerRepository
    {
        private readonly ApplicationDbContext dbcontext;
        public AnswerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbcontext = dbContext;
        }
    }
}
