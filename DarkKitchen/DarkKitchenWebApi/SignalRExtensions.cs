using System.Diagnostics.CodeAnalysis;
using BusinessLogicsAbstractions;
using DarkKitchen.SignalR;

namespace DarkKitchen;

[ExcludeFromCodeCoverage]
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
