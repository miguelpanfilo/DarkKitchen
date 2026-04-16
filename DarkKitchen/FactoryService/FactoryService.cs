using Almacenamiento;
using AlmacenamientoAbstracciones;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryService;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDarkKitchenDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<ISignUpUserService, SignUpService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IModifyUserService, ModifyUserService>();
        services.AddScoped<IDeleteUserService, DeleteUserService>();
        services.AddScoped<IFilterUserService, FilterUserService>();
        services.AddScoped<IAdminValidationService, AdminValidationService>();
        services.AddScoped<ISessionsValidationService, SessionsValidationService>();
        services.AddSingleton<IInitializationService, InitializationService>();
        services.AddScoped<IPhoneValidator, PhoneValidatorUruguay>();
        services.AddScoped<IReloj, WatchSystem>();
        services.AddSingleton<IRepositoryUser, RepositoryUser>();
        services.AddSingleton<IRepositorySession, RepositorySession>();
        return services;
    }
}
