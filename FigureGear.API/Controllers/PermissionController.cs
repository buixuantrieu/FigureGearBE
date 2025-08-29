using Asp.Versioning;
using FigureGear.API.Attributes;
using FigureGear.Service.Interface;
using FigureGear.Service.Models;
using FigureGear.Service.Shared.Filter.Model;
using FigureGear.Service.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace FigureGear.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionValidator _permissionValidator;
        private readonly IPermissionService _permissionService;
        public PermissionController(PermissionValidator permissionValidator, IPermissionService permissionService)
        {
            _permissionValidator = permissionValidator;
            _permissionService = permissionService;
        }

        [RequirePermission("Permission.AddOrUpdate")]
        [HttpPost("AddOrUpdatePermission")]
        public async Task<IActionResult> AddOrUpdatePermission([FromBody] PermissionModel model)
        {
            var validationResult = await _permissionValidator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            var response = await _permissionService.AddOrUpdatePermission(model);

            return StatusCode(response.StatusCode, response);
        }

        [RequirePermission("Permission.Get")]
        [HttpGet]
        public async Task<IActionResult> GetPermissions([FromQuery] PagedFilterRequest filter)
        {
            var response = await _permissionService.GetPermissions(filter);

            return StatusCode(response.StatusCode, response);
        }

        [RequirePermission("Permission.Get")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermission(int id)
        {
            var response = await _permissionService.GetPermission(id);

            return StatusCode(response.StatusCode, response);
        }

        [RequirePermission("Permission.Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var response = await _permissionService.DeletePermission(id);

            return StatusCode(response.StatusCode, response);
        }

        [RequirePermission("Permission.Delete")]
        [HttpDelete("DeleteMultiple")]
        public async Task<IActionResult> DeletePermissions([FromBody] List<int> ids)
        {
            var response = await _permissionService.DeletePermissions(ids);

            return StatusCode(response.StatusCode, response);
        }
    }
}
