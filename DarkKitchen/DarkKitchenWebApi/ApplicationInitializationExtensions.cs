using System.Diagnostics.CodeAnalysis;
using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Users;
using DarkKitchen.Services;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen;

[ExcludeFromCodeCoverage]
public static class ApplicationInitializationExtensions
{
    public static WebApplication InitializeDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<SqlContext>();
        context.Database.Migrate();

        var initializationService = scope.ServiceProvider.GetRequiredService<IInitializationService>();
        initializationService.EnsureInitialized();

        return app;
    }

    public static IServiceCollection AddWebApiServices(this
        IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }

    public static WebApplication UseCorsPolicy(this WebApplication app)
    {
        app.UseCors("AllowAngular");
        return app;
    }
}
