namespace FigureGear.Service.Interface.UserInterface
{
    public interface IUserValidatorService
    {
        Task<bool> IsUserNameExist(string userName);

        Task<bool> IsEmailExist(string email);
    }
}
