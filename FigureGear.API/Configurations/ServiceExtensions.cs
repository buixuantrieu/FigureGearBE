using FigureGear.API.Services.Email;
using FigureGear.Service.Implementation;
using FigureGear.Service.Interface;

namespace FigureGear.API.Configurations
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterService( this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }
    }
}
