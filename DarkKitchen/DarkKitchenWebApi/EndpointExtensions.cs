using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Hubs;

namespace DarkKitchen;

[ExcludeFromCodeCoverage]
public static class EndpointExtensions
{
    public static WebApplication MapApplicationEndpoints(
        this WebApplication app)
    {
        app.MapControllers();

        app.MapHub<ProductImportersHub>(
            "/hubs/product-importers");

        return app;
    }
}
