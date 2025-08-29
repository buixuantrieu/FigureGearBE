using FigureGear.API.Services.Email;
using FigureGear.Service.Implementation;
using FigureGear.Service.Implementation.UserImplementation;
using FigureGear.Service.Interface;
using FigureGear.Service.Interface.UserInterface;

namespace FigureGear.API.Configurations
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterService( this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IUserValidatorService, UserValidatorService>();
            
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPermissionService, PermissionService>();
            

            return services;
        }
    }
}
