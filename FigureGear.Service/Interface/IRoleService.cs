using FigureGear.Service.Models;
using FigureGear.Service.Shared.Filter.Model;

namespace FigureGear.Service.Interface
{
    public interface IRoleService
    {
        Task<ApiResponse<dynamic>> AddOrUpdateRole(RoleModel model);

        Task<ApiResponse<dynamic>> GetRoles(PagedFilterRequest filter);

        Task<ApiResponse<dynamic>> GetRole(int id);

        Task<ApiResponse<dynamic>> DeleteRole(int id);
    }
}
