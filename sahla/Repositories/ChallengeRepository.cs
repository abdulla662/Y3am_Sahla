using sahla.DataAcess;
using sahla.Models;
using sahla.Repositories.Ireposotries;

namespace sahla.Repositories
{
    public class ChallengeRepository : Repository<Challenge>, IChallengeRepository
    {
        private readonly ApplicationDbContext dbcontext;
        public ChallengeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbcontext = dbContext;
        }

    }
}

