using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryService;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDarkKitchenDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}
