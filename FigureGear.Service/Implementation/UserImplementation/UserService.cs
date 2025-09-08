using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FigureGear.Data.Context;
using FigureGear.Data.Domain;
using FigureGear.Service.Interface;
using FigureGear.Service.Models;
using FigureGear.Service.Shared;
using FigureGear.Service.Helpers;
using FigureGear.Service.Interface.UserInterface;

namespace FigureGear.Service.Implementation.UserImplementation
{
    public class UserService : DbContextService, IUserService
    {
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;

        public UserService(FigureGearDbContext dbContext, IMapper mapper, ITokenService tokenService, IConfiguration configuration, IEmailService emailService, ICurrentUserService currentUserService) : base(dbContext)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _configuration = configuration;
            _emailService = emailService;
            _currentUserService = currentUserService;
        }

        #region Register
        public async Task<ApiResponse<dynamic>> RegisterAsync(UserModel model)
        {
            var user = _mapper.Map<User>(model)!;
            user.Id = Guid.NewGuid();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var defaultRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.User);
            if (defaultRole == null)
            {
                return ApiResponse<dynamic>.NotFound("default role not found");
            }
            user.RoleId = defaultRole.Id;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var userIdEncrypt = Utils.Encrypt(user.Id.ToString());
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Email", "ActivationEmailTemplate.html");
            var emailTemplate = await File.ReadAllTextAsync(templatePath);
            emailTemplate = emailTemplate
                .Replace("{{logoUrl}}", "https://firebasestorage.googleapis.com/v0/b/sdsd-f6fec.appspot.com/o/images%2FLogo.png?alt=media&token=bd150de3-94be-4c84-b177-0f799f6dd955")
                .Replace("{{confirmLink}}", $"{_configuration["AppSettings:ClientUrl"]}/auth/confirm-email?token={userIdEncrypt}")
                .Replace("{{currentYear}}", DateTime.Now.Year.ToString());

            await _emailService.SendEmailAsync(user.Email, "Xác thực tài khoản FigureGear Store", emailTemplate);

            return ApiResponse<dynamic>.Created("user registered successfully");
        }
        #endregion

        #region Confirm email
        public async Task<ApiResponse<dynamic>> ConfirmEmailAsync(string token)
        {
            var userId = Utils.Decrypt(token);
            var user = _dbContext.Users
                        .Include(u => u.Role)
                            .ThenInclude(r => r.RolePermissions)
                                .ThenInclude(rp => rp.Permission)
                        .FirstOrDefault(u => u.Id == Guid.Parse(userId));

            if (user == null || user.EmailConfirmed)
            {
                return ApiResponse<dynamic>.BadRequest("invalid credentials");
            }

            user.SecurityStamp = Guid.NewGuid().ToString();

            var permissions = user.Role?.RolePermissions.Select(rp => rp.Permission.Key).ToList() ?? new List<string>();

            var authClaims = BuildClaims(user, permissions);

            var refreshToken = _tokenService.GenerateRefreshToken();
            var accessToken = _tokenService.GenerateAccessToken(authClaims);

            user.EmailConfirmed = true;
            user.LastLoginDate = DateTime.UtcNow;
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            user.UserProfile ??= new UserProfile { UserId = user.Id };

            await _dbContext.SaveChangesAsync();

            return ApiResponse<dynamic>.Ok("email confirmed successfully", new
            {
                Token = $"{accessToken}.{_configuration["JWT:concatString"]}.{refreshToken}"
            });

        }
        #endregion

        #region Login
        public async Task<ApiResponse<dynamic>> LoginAsync(UserModel model)
        {
            var user = _dbContext.Users
                         .Include(u => u.Role)
                         .ThenInclude(r => r.RolePermissions)
                         .ThenInclude(rp => rp.Permission)
                         .FirstOrDefault(u => u.UserName == model.UserName);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return ApiResponse<dynamic>.BadRequest("wrong username or password");
            }

            if (!user.EmailConfirmed)
            {
                return ApiResponse<dynamic>.BadRequest("account email not verified");
            }

            var permissions = user.Role?.RolePermissions
                               .Select(rp => rp.Permission.Key)
                               .Distinct()
                               .ToList() ?? new List<string>();
            user.SecurityStamp = Guid.NewGuid().ToString();

            var authClaims = BuildClaims(user, permissions);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var accessToken = _tokenService.GenerateAccessToken(authClaims);

            user.LastLoginDate = DateTime.UtcNow;
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _dbContext.SaveChangesAsync();

            return ApiResponse<dynamic>.Ok("logged in successfully", new
            {
                Token = $"{accessToken}.{_configuration["JWT:concatString"]}.{refreshToken}"
            });
        }
        #endregion

        #region Get user info
        public async Task<ApiResponse<dynamic>> GetUserInfo()
        {
            var currentUserId = _currentUserService.UserId!.Value;
            var user = await _dbContext.Users.Include(u => u.UserProfile)
                                             .Where(u => u.Id == currentUserId)
                                             .Select(u => new
                                             {
                                                 email = u.Email,
                                                 userName = u.UserName,
                                                 avatar = u.UserProfile.AvatarUrl,
                                                 address = u.UserProfile.Address,
                                                 dateOfBirth = u.UserProfile.DateOfBirth,
                                                 phoneNumber = u.UserProfile.PhoneNumber,
                                                 fullName = u.UserProfile.FullName
                                             }).SingleOrDefaultAsync();

            if (user == null)
            {
                return ApiResponse<dynamic>.NotFound("User not found");
            }

            return ApiResponse<dynamic>.Ok("Get user info successfully", user);
        }
        #endregion

        #region Helpers
        private static List<Claim> BuildClaims(User user, List<string> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("SecurityStamp", user.SecurityStamp)
            };
            claims.AddRange(permissions.Select(p => new Claim("Permission", p)));
            return claims;
        }

        #endregion
    }
}
