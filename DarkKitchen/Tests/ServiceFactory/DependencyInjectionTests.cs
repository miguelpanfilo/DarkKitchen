using BusinessLogics;
using BusinessLogicsAbstractions;
using DataAccess.Repositories;
using DataAccessAbstractions;
using FactoryService;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

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

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersOrderRepository()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IOrderRepository));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(OrderRepository), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersProductRepository()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IProductRepository));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(ProductRepository), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersPromotionRepository()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IPromotionRepository));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(PromotionRepository), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersOrderFacade()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IOrderFacade));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(OrderFacade), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersOrderService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IOrderService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(OrderService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersReportService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IReportService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(ReportService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersProductService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IProductService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(ProductService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersPromotionService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IPromotionService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(PromotionService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersShippingTypeService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IShippingTypeService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(ShippingTypeService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersAuditLogRepository()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IAuditLogRepository));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(AuditLogRepository), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersAuditLogService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IAuditLogService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(AuditLogService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersProductImporterQueryService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IProductImporterQueryService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(ProductImporterQueryService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersProductImportService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IProductImportService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(ProductImportService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_RegistersProductImportersChangeService()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddDarkKitchenDependencies(configuration);

        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IProductImportersChangeService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(ProductImportersChangeService), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_CanResolveProductImporterProvider()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["PhotoStorage:Path"] = Path.GetTempPath() })
            .Build();
        services.AddDarkKitchenDependencies(configuration);
        services.AddSingleton<IProductImportersChangeNotifier>(new Mock<IProductImportersChangeNotifier>().Object);

        var provider = services.BuildServiceProvider();

        var result = provider.GetRequiredService<IProductImporterProvider>();

        result.Should().NotBeNull();
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_CanResolveProductImportersWatcher()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["PhotoStorage:Path"] = Path.GetTempPath() })
            .Build();
        services.AddDarkKitchenDependencies(configuration);
        services.AddSingleton<IProductImportersChangeNotifier>(new Mock<IProductImportersChangeNotifier>().Object);

        var provider = services.BuildServiceProvider();

        var result = provider.GetRequiredService<ProductImportersWatcher>();

        result.Should().NotBeNull();
    }

    [TestMethod]
    public void AddDarkKitchenDependencies_CanResolveProductPhotoStorage()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["PhotoStorage:Path"] = Path.GetTempPath() })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddDarkKitchenDependencies(configuration);

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var result = scope.ServiceProvider.GetRequiredService<IProductPhotoStorage>();

        result.Should().NotBeNull();
    }
}
