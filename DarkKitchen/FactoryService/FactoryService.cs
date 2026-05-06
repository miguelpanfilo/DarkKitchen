using BusinessLogics;
using BusinessLogics.Initialization;
using BusinessLogics.Orders;
using BusinessLogics.Products;
using BusinessLogics.Promotions;
using BusinessLogics.Reports;
using BusinessLogics.Sessions;
using BusinessLogics.Shared;
using BusinessLogics.Users;
using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Orders;
using BusinessLogicsAbstractions.Products;
using BusinessLogicsAbstractions.Promotions;
using BusinessLogicsAbstractions.Reports;
using BusinessLogicsAbstractions.Users;
using DataAccess;
using DataAccess.Repositories;
using DataAccessAbstractions;
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
        services.AddAuditServices();

        return services;
    }

    private static IServiceCollection AddAuditServices(this IServiceCollection
        services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditLogService, AuditLogService>();

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
        services.AddScoped<IOrderItemResolver, OrderItemResolver>();

        services.AddScoped<IReportService, ReportService>();

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
