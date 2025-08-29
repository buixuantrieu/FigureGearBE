using FigureGear.Service.Models;

namespace FigureGear.Service.Interface.UserInterface
{
    public interface IUserService
    {
        Task<ApiResponse<dynamic>> RegisterAsync(UserModel model);

        Task<ApiResponse<dynamic>> LoginAsync(UserModel model);

        Task<ApiResponse<dynamic>> ConfirmEmailAsync(string token);
    }
}
