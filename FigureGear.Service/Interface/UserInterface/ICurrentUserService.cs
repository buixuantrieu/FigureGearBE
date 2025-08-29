namespace FigureGear.Service.Interface.UserInterface
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}
