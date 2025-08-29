using FigureGear.Service.Interface.UserInterface;
using FigureGear.Service.Models;
using FluentValidation;

namespace FigureGear.Service.Validators
{
    public class UserValidator: AbstractValidator<UserModel>
    {
        private readonly IUserValidatorService _userValidatorService;

        public UserValidator(IUserValidatorService userValidatorService)
        {
            _userValidatorService = userValidatorService;

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("user name is required")
                .MaximumLength(50).WithMessage("user name must not exceed 50 characters")
                .MustAsync(async (user, userName, cancellation) => !await _userValidatorService.IsUserNameExist(userName)).WithMessage("user name is already exist");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("email is required")
                .EmailAddress().WithMessage("invalid email format")
                .MaximumLength(50).WithMessage("email must not exceed 50 characters")
                .MustAsync(async (user, email, cancellation)=> !await _userValidatorService.IsEmailExist(email)).WithMessage("email is already exist");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("password is required")
                .MaximumLength(100).WithMessage("password must not exceed 100 characters");
        }
    }
}
