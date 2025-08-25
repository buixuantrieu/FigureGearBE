using FigureGear.Service.Models;

namespace FigureGear.Service.Interface
{
    public interface IUserService
    {
        Task<ApiResponse<dynamic>> RegisterAsync(UserModel model);

        Task<ApiResponse<dynamic>> LoginAsync(UserModel model);

        Task<ApiResponse<dynamic>> ConfirmEmailAsync(string token);

        #region Validators
        Task<bool> IsUserNameExist(string userName);

        Task<bool> IsEmailExist(string email);
        #endregion
    }
}
