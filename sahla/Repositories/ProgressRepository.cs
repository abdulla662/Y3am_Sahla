using sahla.DataAcess;
using sahla.Models;
using sahla.Repositories.Ireposotries;

namespace sahla.Repositories
{
    public class ProgressRepository : Repository<Progress>, IProgressRepository
    {
        private readonly ApplicationDbContext dbcontext;
        public ProgressRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbcontext = dbContext;
        }

    }
}

