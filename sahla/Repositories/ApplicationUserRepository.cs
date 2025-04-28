using sahla.DataAcess;
using sahla.Models;
using sahla.Repositories.Ireposotries;

namespace sahla.Repositories
{
    public class ApplicationUserRepository: Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext dbcontext;
        public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbcontext = dbContext;
        }
    
    }
}
