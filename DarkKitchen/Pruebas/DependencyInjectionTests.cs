using FactoryService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pruebas;

[TestClass]
public class DependencyInjectionTests
{
    [TestMethod]
    public void AddDarkKitchenDependencies_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var result = services.AddDarkKitchenDependencies(configuration);

        Assert.AreSame(services, result);
    }
}
