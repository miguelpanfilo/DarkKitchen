using Almacenamiento;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Users;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchenWebApi.Extensions;

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
