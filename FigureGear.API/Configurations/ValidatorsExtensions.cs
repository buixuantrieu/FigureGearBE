using FigureGear.Service.Validators;

namespace FigureGear.API.Configurations
{
    public static class ValidatorsExtensions
    {
        public static IServiceCollection RegisterValidators(this IServiceCollection services)
        {
            services.AddScoped<UserValidator>();
            services.AddScoped<PermissionValidator>();

            return services;
        }
    }
}
