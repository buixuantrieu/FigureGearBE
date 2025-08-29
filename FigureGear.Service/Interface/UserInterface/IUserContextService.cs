namespace FigureGear.Service.Interface.UserInterface
{
    public interface IUserContextService
    {
        Task<(string RoleName, string SecurityStamp)?> GetUserInfoAsync(Guid userId);
    }
}
