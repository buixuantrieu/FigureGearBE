using FigureGear.Service.Interface;
using FigureGear.Service.Models;
using FluentValidation;

namespace FigureGear.Service.Validators
{
    public class PermissionValidator : AbstractValidator<PermissionModel>
    {
        private readonly IPermissionService _isPermissionService;
        public PermissionValidator(IPermissionService isPermissionService) {
            _isPermissionService = isPermissionService;
        
            RuleFor(x=>x.Key).NotEmpty().WithMessage("permission name is required")
                             .MaximumLength(100).WithMessage("permission name must not exceed 50 characters")
                             .MustAsync(async (permission, key, cancellation) => !await _isPermissionService.IsKeyExist(permission)).WithMessage("permission name is already exist");

            RuleFor(x => x.Description).MaximumLength(250).WithMessage("permission name must not exceed 250 characters");
        }
    }
}
