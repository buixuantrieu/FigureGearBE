using FigureGear.Service.Models;
using FluentValidation;

namespace FigureGear.Service.Validators
{
    public class UserValidator: AbstractValidator<UserModel>
    {
        public UserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required")
                .MaximumLength(50);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MaximumLength(100);

            RuleFor(x => x.RefreshTokenExpiryTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("Expiry time must be in the future");
        }
    }
}
