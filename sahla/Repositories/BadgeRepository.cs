using sahla.DataAcess;
using sahla.Models;
using sahla.Repositories.Ireposotries;

namespace sahla.Repositories
{
    public class BadgeRepository : Repository<Badge>, IBadgeRepository
    {
        private readonly ApplicationDbContext dbcontext;
        public BadgeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbcontext = dbContext;
        }

    }
}


