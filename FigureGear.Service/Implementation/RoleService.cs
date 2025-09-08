using AutoMapper;
using FigureGear.Data.Context;
using FigureGear.Data.Domain;
using FigureGear.Service.Helpers;
using FigureGear.Service.Interface;
using FigureGear.Service.Interface.UserInterface;
using FigureGear.Service.Models;
using FigureGear.Service.Shared.Filter;
using FigureGear.Service.Shared.Filter.Model;
using Microsoft.EntityFrameworkCore;

namespace FigureGear.Service.Implementation
{
    public class RoleService : DbContextService, IRoleService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _iMapper;

        public RoleService(FigureGearDbContext dbContext, IMapper iMapper, ICurrentUserService currentUserService) : base(dbContext)
        {
            _currentUserService = currentUserService;
            _iMapper = iMapper;
        }

        public async Task<ApiResponse<dynamic>> AddOrUpdateRole(RoleModel model)
        {
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == model.Id);
            var currentUserId = _currentUserService.UserId!.Value;
            var isNew = role == null;

            if (isNew)
            {
                role = _iMapper.Map<Role>(model);
                role.CreatedBy = currentUserId;
                _dbContext.Roles.Add(role);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                role.Name = model.Name;
                role.Description = model.Description;
                role.UpdatedDate = DateTime.UtcNow;
                role.IsActive = model.IsActive;
                await _dbContext.SaveChangesAsync();
            }
            if (model.PermissionIds != null)
            {
                var currentPermissionIds = await _dbContext.RolePermissions.Where(rp => rp.RoleId == role.Id)
                                                                           .Select(rp => rp.PermissionId)
                                                                           .ToListAsync();
                var toAdd = model.PermissionIds.Except(currentPermissionIds);

                foreach (var permissionId in toAdd)
                {
                    _dbContext.RolePermissions.Add(new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permissionId
                    });
                }

                var toRemove = currentPermissionIds.Except(model.PermissionIds);
                var removeRolePermissions = _dbContext.RolePermissions.Where(rp => rp.RoleId == role.Id && toRemove.Contains(rp.PermissionId));
                _dbContext.RolePermissions.RemoveRange(removeRolePermissions);

                await _dbContext.SaveChangesAsync();
            }

            var message = isNew ? "role created successfully" : "role updated successfully";

            return isNew ? ApiResponse<dynamic>.Created(message, role) : ApiResponse<dynamic>.Ok(message, role);

        }

        public async Task<ApiResponse<dynamic>> GetRoles(PagedFilterRequest filter)
        {
            var roles = await _dbContext.Roles.Where(r => r.Name != RoleNames.Admin && r.Name != RoleNames.User)
                                              .Select(r => new RoleModel
                                              {
                                                  Id = r.Id,
                                                  Name = r.Name,
                                                  Description = r.Description,
                                                  Permissions = r.RolePermissions.Select(rp => new PermissionModel
                                                  {
                                                      Id = rp.Permission.Id,
                                                      Key = rp.Permission.Key
                                                  }).ToList()
                                              }).ApplyAdvancedFilterAsync(filter);

            return ApiResponse<dynamic>.Ok("get roles successfully", roles);
        }

        public async Task<ApiResponse<dynamic>> GetRole(int id)
        {
            var role = await _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return ApiResponse<dynamic>.NotFound("role not found");

            return ApiResponse<dynamic>.Ok("get role successfully", role);
        }

        public async Task<ApiResponse<dynamic>> DeleteRole(int id)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var roleToDelete = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
                if (roleToDelete == null)
                    return ApiResponse<dynamic>.NotFound("role not found");

                var defaultRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.User);

                if (defaultRole == null)
                    return ApiResponse<dynamic>.BadRequest("default role 'User' not found");

                var usersWithRole = await _dbContext.Users.Where(u => u.RoleId == roleToDelete.Id)
                                                          .ToListAsync();
                usersWithRole.ForEach(u => u.RoleId = defaultRole.Id);

                await _dbContext.SaveChangesAsync();
                _dbContext.Roles.Remove(roleToDelete);
                await transaction.CommitAsync();

                return ApiResponse<dynamic>.Ok("role deleted successfully and users updated to default role");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
