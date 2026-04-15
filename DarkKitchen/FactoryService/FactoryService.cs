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

        return services;
    }
}
