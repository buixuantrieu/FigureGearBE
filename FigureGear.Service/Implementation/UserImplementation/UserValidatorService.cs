using FigureGear.Data.Context;
using FigureGear.Service.Interface.UserInterface;
using Microsoft.EntityFrameworkCore;

namespace FigureGear.Service.Implementation.UserImplementation
{
    public class UserValidatorService :DbContextService, IUserValidatorService
    {
        public UserValidatorService(FigureGearDbContext dbContext):base(dbContext) { }

        public async Task<bool> IsUserNameExist(string userName) =>
          await _dbContext.Users.AsNoTracking().AnyAsync(user => user.UserName == userName);

        public async Task<bool> IsEmailExist(string email) =>
            await _dbContext.Users.AsNoTracking().AnyAsync(user => user.Email == email);
    }
}
