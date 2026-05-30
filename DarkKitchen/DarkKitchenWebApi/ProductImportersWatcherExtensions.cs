using BusinessLogics;

namespace DarkKitchen;

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
