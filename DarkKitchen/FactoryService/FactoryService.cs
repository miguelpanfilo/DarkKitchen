using Almacenamiento;
using Almacenamiento.Repositories;
using AlmacenamientoAbstracciones;
using LogicaDeNegocio.Initialization;
using LogicaDeNegocio.Orders;
using LogicaDeNegocio.Products;
using LogicaDeNegocio.Promotions;
using LogicaDeNegocio.Sessions;
using LogicaDeNegocio.Shared;
using LogicaDeNegocio.Users;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Orders;
using LogicaDeNegocioAbstracciones.Products;
using LogicaDeNegocioAbstracciones.Promotions;
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
        services.AddPersistence(configuration);
        services.AddUserServices();
        services.AddOrderServices();
        services.AddSharedServices();
        services.AddInitializationServices();

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SqlContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IRepositoryUser, RepositoryUser>();
        services.AddScoped<IRepositorySession, RepositorySession>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();

        services.AddScoped<IProductPhotoStorage>(_ => new ProductPhotoStorage("C:\\temp\\photos"));

        return services;
    }

    private static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<ISignUpUserService, SignUpService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IModifyUserService, ModifyUserService>();
        services.AddScoped<IDeleteUserService, DeleteUserService>();
        services.AddScoped<IFilterUserService, FilterUserService>();
        services.AddScoped<IAdminValidationService, AdminValidationService>();
        services.AddScoped<ISessionsValidationService, SessionsValidationService>();

        return services;
    }

    private static IServiceCollection AddOrderServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderFacade, OrderFacade>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<ProductPhotoService>();

        return services;
    }

    private static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<IPhoneValidator, PhoneValidatorUruguay>();
        services.AddScoped<IWatch, WatchSystem>();

        return services;
    }

    private static IServiceCollection AddInitializationServices(this IServiceCollection services)
    {
        services.AddScoped<IInitializationService, InitializationService>();

        return services;
    }
}
