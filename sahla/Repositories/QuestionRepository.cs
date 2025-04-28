using sahla.DataAcess;
using sahla.Models;
using sahla.Repositories.Ireposotries;

namespace sahla.Repositories
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        private readonly ApplicationDbContext dbcontext;
        public QuestionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbcontext = dbContext;
        }

    }
}

