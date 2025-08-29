using AutoMapper;
using FigureGear.Data.Context;
using FigureGear.Data.Domain;
using FigureGear.Service.Interface;
using FigureGear.Service.Interface.UserInterface;
using FigureGear.Service.Models;
using FigureGear.Service.Shared.Filter;
using FigureGear.Service.Shared.Filter.Model;
using Microsoft.EntityFrameworkCore;

namespace FigureGear.Service.Implementation
{
    public class PermissionService : DbContextService, IPermissionService
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

    
        public PermissionService(FigureGearDbContext dbContext, IMapper mapper, ICurrentUserService currentUserService) : base(dbContext)
        {
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        #region CRUD
        public async Task<ApiResponse<dynamic>> AddOrUpdatePermission(PermissionModel model)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == model.Id);
            var currentUserId = _currentUserService.UserId!.Value;
            bool isNew = permission == null;

            if (isNew)
            {
                permission = _mapper.Map<Permission>(model);
                permission.CreatedBy = currentUserId;
                _dbContext.Permissions.Add(permission);
            }
            else
            {
                permission.Key = model.Key;
                permission.Description = model.Description;
                permission.UpdatedDate = DateTime.UtcNow;
                permission.UpdatedBy = currentUserId;
            }

            await _dbContext.SaveChangesAsync();

            var message = isNew ? "permission created successfully" : "permission updated successfully";

            return isNew ? ApiResponse<dynamic>.Created(message, permission) : ApiResponse<dynamic>.Ok(message, permission);
        }

        public async Task<ApiResponse<dynamic>> GetPermissions(PagedFilterRequest request)
        {
            var result = await _dbContext.Permissions.ApplyAdvancedFilterAsync(request);

            return ApiResponse<dynamic>.Ok("get permissions successfully", result);
        }

        public async Task<ApiResponse<dynamic>> GetPermission(int id)
        {
            var permission = await _dbContext.Permissions.AsNoTracking()
                                                         .FirstOrDefaultAsync(p=> p.Id == id);

            if (permission == null)
            {
                return ApiResponse<dynamic>.NotFound("permission not found");
            }

            return ApiResponse<dynamic>.Ok("get permission successfully", permission);
        }

        public async Task<ApiResponse<dynamic>> DeletePermission(int id)
        {
            var affected = await _dbContext.Permissions.Where(p => p.Id == id).ExecuteDeleteAsync();

            if (affected == 0)
                return ApiResponse<dynamic>.NotFound("permission not found");

            return ApiResponse<dynamic>.Ok("permission deleted successfully");
        }

        public async Task<ApiResponse<dynamic>> DeletePermissions(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return ApiResponse<dynamic>.BadRequest("no permission ids provided");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                await _dbContext.RolePermissions.Where(rp => ids.Contains(rp.PermissionId)).ExecuteDeleteAsync();

                var affectedPermissions = await _dbContext.Permissions.Where(p => ids.Contains(p.Id)).ExecuteDeleteAsync();

                if (affectedPermissions == 0)
                    return ApiResponse<dynamic>.NotFound("no permissions found for the provided ids");

                await transaction.CommitAsync();
                return ApiResponse<dynamic>.Ok("permissions deleted successfully");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion

        #region Validator
        public async Task<bool> IsKeyExist(PermissionModel model) =>
        await _dbContext.Permissions.AsNoTracking().AnyAsync(p => p.Key.ToLower() == model.Key.ToLower() && model.Id != p.Id);
        #endregion
    }
}
