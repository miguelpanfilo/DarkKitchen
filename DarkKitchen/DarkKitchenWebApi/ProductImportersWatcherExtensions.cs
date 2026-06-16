using System.Diagnostics.CodeAnalysis;
using BusinessLogics;

namespace DarkKitchen;

[ExcludeFromCodeCoverage]
public static class ProductImportersWatcherExtensions
{
    public static WebApplication StartProductImportersWatcher(
        this WebApplication app)
    {
        var watcher = app.Services.GetRequiredService<ProductImportersWatcher>();

        watcher.Start();

        return app;
    }
}
