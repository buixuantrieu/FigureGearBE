
using FigureGear.Data.Context;

namespace FigureGear.Service.Implementation
{
    public class DbContextService
    {
        protected readonly FigureGearDbContext _dbContext;

        public DbContextService(FigureGearDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
