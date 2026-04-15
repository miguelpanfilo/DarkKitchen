using FactoryService;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
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

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersLoginService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(ILoginService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(LoginService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersSignUpUserService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(ISignUpUserService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(SignUpService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }
}
