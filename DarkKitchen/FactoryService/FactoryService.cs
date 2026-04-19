using Almacenamiento;
using Almacenamiento.Repositories;
using AlmacenamientoAbstracciones;
using LogicaDeNegocio;
using LogicaDeNegocio.Initialization;
using LogicaDeNegocio.Sessions;
using LogicaDeNegocio.Shared;
using LogicaDeNegocio.Users;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryService;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDarkKitchenDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SqlContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<ISignUpUserService, SignUpService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IModifyUserService, ModifyUserService>();
        services.AddScoped<IDeleteUserService, DeleteUserService>();
        services.AddScoped<IFilterUserService, FilterUserService>();
        services.AddScoped<IAdminValidationService, AdminValidationService>();
        services.AddScoped<ISessionsValidationService, SessionsValidationService>();

        services.AddScoped<IInitializationService, InitializationService>();

        services.AddScoped<IPhoneValidator, PhoneValidatorUruguay>();
        services.AddScoped<IWatch, WatchSystem>();

        services.AddScoped<IRepositoryUser, RepositoryUser>();
        services.AddScoped<IRepositorySession, RepositorySession>();
        return services;
    }
}
