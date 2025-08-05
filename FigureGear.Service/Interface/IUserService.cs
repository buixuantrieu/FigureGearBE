
using FigureGear.Service.DTO;
using FigureGear.Service.Models;

namespace FigureGear.Service.Interface
{
    public interface IUserService
    {
        Task<ApiResponse<object>> RegisterAsync(UserModel model);

        Task<AuthResponse> ConfirmEmailAsync(string token);

        #region Validators
        Task<bool> IsUserNameExist(string userName);

        Task<bool> IsEmailExist(string email);
        #endregion
    }
}
