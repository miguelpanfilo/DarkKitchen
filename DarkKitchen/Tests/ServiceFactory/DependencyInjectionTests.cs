using BusinessLogics.Initialization;
using BusinessLogics.Sessions;
using BusinessLogics.Shared;
using BusinessLogics.Users;
using BusinessLogicsAbstractions.Users;
using DataAccess.Repositories;
using DataAccessAbstractions;
using FactoryService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.ServiceFactory;

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

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersUserService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IUserService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(UserService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersModifyUserService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IModifyUserService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(ModifyUserService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersDeleteUserService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IDeleteUserService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(DeleteUserService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersFilterUserService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IFilterUserService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(FilterUserService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersAdminValidationService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IAdminValidationService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(AdminValidationService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersSessionsValidationService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(ISessionsValidationService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(SessionsValidationService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersInitializationService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IInitializationService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(InitializationService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersPhoneValidator()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IPhoneValidator));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(PhoneValidatorUruguay), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersWatchSystem()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IWatch));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(WatchSystem), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersUserRepository()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IRepositoryUser));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(RepositoryUser), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersSessionRepository()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IRepositorySession));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(RepositorySession), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersShippingTypeRepository()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IShippingTypeRepository));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(ShippingTypeRepository), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }
}
