using FigureGear.Service.Models;
using FigureGear.Service.Shared.Filter.Model;

namespace FigureGear.Service.Interface
{
    public interface IPermissionService
    {
        Task<ApiResponse<dynamic>> AddOrUpdatePermission(PermissionModel model);

        Task<ApiResponse<dynamic>> GetPermissions(PagedFilterRequest request);

        Task<ApiResponse<dynamic>> GetPermission(int id);

        Task<ApiResponse<dynamic>> DeletePermission(int id);

        Task<ApiResponse<dynamic>> DeletePermissions(List<int> ids);

        Task<bool> IsKeyExist(PermissionModel model);
    }
}
