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

        ReseedIdentities(context);

        return app;
    }

    private static void ReseedIdentities(SqlContext context)
    {
        var tables = new (string Table, int MaxId)[]
        {
            ("Users",         context.Users.Any()         ? context.Users.Max(u => u.Id)         : 0),
            ("Products",      context.Products.Any()      ? context.Products.Max(p => p.Id)      : 0),
            ("Promotions",    context.Promotions.Any()    ? context.Promotions.Max(p => p.Id)    : 0),
            ("Orders",        context.Orders.Any()        ? context.Orders.Max(o => o.Id)        : 0),
            ("AuditLogs",     context.AuditLogs.Any()     ? context.AuditLogs.Max(a => a.Id)     : 0),
            ("ShippingTypes", context.ShippingTypes.Any() ? context.ShippingTypes.Max(s => s.Id) : 0),
        };

        foreach (var (table, maxId) in tables)
        {
            if (maxId > 0)
            {
                context.Database.ExecuteSqlRaw(
                    $"DBCC CHECKIDENT ('{table}', RESEED, {maxId})");
            }
        }
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
