using BusinessLogicsAbstractions.Imports;
using DarkKitchen.SignalR;

namespace DarkKitchen;

public static class SignalRExtensions
{
    public static IServiceCollection AddSignalRServices(
        this IServiceCollection services)
    {
        services.AddSignalR();

        services.AddSingleton<IProductImportersChangeNotifier,
            ProductImportersSignalRNotifier>();

        return services;
    }
}
