using System.Security.Claims;
using FigureGear.Data.Context;
using FigureGear.Data.Domain;
using FigureGear.Service.DTO;
using FigureGear.Service.Interface;
using FigureGear.Service.Models;
using FigureGear.Service.Shared;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using FigureGear.Service.Helpers;

namespace FigureGear.Service.Implementation
{
    public class UserService : DbContextService, IUserService
    {
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UserService(FigureGearDbContext dbContext, IMapper mapper, ITokenService tokenService, IConfiguration configuration, IEmailService emailService) : base(dbContext)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<ApiResponse<object>> RegisterAsync(UserModel model)
        {
            var user = _mapper.Map<User>(model)!;

            user.Id = Guid.NewGuid();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var userIdEncrypt = Utils.Encrypt(user.Id.ToString());
            var defaultRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.User);
            if (defaultRole == null) {
                throw new Exception("Default role not found");
            }
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = defaultRole.Id
            });
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Email", "ActivationEmailTemplate.html");
            var emailTemplate = await File.ReadAllTextAsync(templatePath);
            emailTemplate = emailTemplate
                      .Replace("{{logoUrl}}", "https://firebasestorage.googleapis.com/v0/b/sdsd-f6fec.appspot.com/o/images%2FLogo.png?alt=media&token=bd150de3-94be-4c84-b177-0f799f6dd955")
                      .Replace("{{confirmLink}}", $"{_configuration["AppSettings:ClientUrl"]}/auth/confirm-email?token={userIdEncrypt}")
                      .Replace("{{currentYear}}", DateTime.Now.Year.ToString());
            await _emailService.SendEmailAsync(model.Email, "Xác thực tài khoản FigureGear Store", emailTemplate);

            return new ApiResponse<object>
            {
                Success = true,
                Message = "Registered successfully"
            };
        }


        public async Task<AuthResponse> ConfirmEmailAsync(string token)
        {
            var userId = Utils.Decrypt(token);

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null || user.EmailConfirmed)
            {
                throw new Exception("the token is invalid.");
            }

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, Utils.Encrypt(user.Id.ToString())),
                    new Claim("SecurityStamp", user.SecurityStamp)
                };


            var refreshToken = _tokenService.GenerateRefreshToken();
            var accessToken = _tokenService.GenerateAccessToken(authClaims);

            user.EmailConfirmed = true;
            user.LastLoginDate = DateTime.UtcNow;
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            string tokenConcat = $"{accessToken}.{_configuration["JWT:concatString"]}.{refreshToken}";

            await _dbContext.SaveChangesAsync();

            return new AuthResponse
            {
                Token = tokenConcat
            };
        }

        #region Validators
        public async Task<bool> IsUserNameExist(string userName)
        {
            var userNameExist = await _dbContext.Users.AsNoTracking().AnyAsync(user => user.UserName == userName);
            return userNameExist;
        }
        public async Task<bool> IsEmailExist(string email)
        {
            var emailExist = await _dbContext.Users.AsNoTracking().AnyAsync(user => user.Email == email);
            return emailExist;
        }
        #endregion
    }
}
