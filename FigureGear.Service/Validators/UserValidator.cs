using FigureGear.Service.Interface;
using FigureGear.Service.Models;
using FluentValidation;

namespace FigureGear.Service.Validators
{
    public class UserValidator: AbstractValidator<UserModel>
    {
        private readonly IUserService _iUser;

        public UserValidator(IUserService iUserService)
        {
            _iUser = iUserService;

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("user name is required")
                .MaximumLength(50).WithMessage("user name must not exceed 50 characters")
                .MustAsync(async (user, userName, cancellation) => !await _iUser.IsUserNameExist(userName)).WithMessage("user name is already exist");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("email is required")
                .EmailAddress().WithMessage("invalid email format")
                .MaximumLength(50).WithMessage("email must not exceed 100 characters")
                .MustAsync(async (user, email, cancellation)=> !await _iUser.IsEmailExist(email)).WithMessage("email is already exist");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("password is required")
                .MaximumLength(100).WithMessage("password must not exceed 100 characters");
        }
    }
}
