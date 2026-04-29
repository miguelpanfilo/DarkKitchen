using System.Diagnostics.CodeAnalysis;
using Almacenamiento;
using LogicaDeNegocioAbstracciones.Users;
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
}
