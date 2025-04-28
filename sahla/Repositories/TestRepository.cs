using sahla.DataAcess;
using sahla.Models;
using sahla.Repositories.Ireposotries;

namespace sahla.Repositories
{
    public class TestRepository : Repository<Test>, ITestRepository
    {
        private readonly ApplicationDbContext dbcontext;
        public TestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbcontext = dbContext;
        }

    }
}

