using BusinessLogicsAbstractions.Imports;
using DarkKitchen.SignalR;

namespace DarkKitchen;

public static class SignalRExtensions
{
    public static IServiceCollection AddSignalRServices(
        this IServiceCollection services)
    {
        services.AddSignalR();

        services.AddScoped<IProductImportersChangeNotifier,
            ProductImportersSignalRNotifier>();

        return services;
    }
}
