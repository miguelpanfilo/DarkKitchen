using BusinessLogics;
using BusinessLogicsAbstractions;
using FluentAssertions;
using Moq;

namespace Tests.imports;

[TestClass]
public class ProductImportersChangeServiceTest
{
    [TestMethod]
    public async Task NotifyImportersChanged_WhenCalled_ShouldReloadImportersAndNotify()
    {
        var provider = new FakeProductImporterProvider();
        var notifier = new FakeProductImportersChangeNotifier();

        var service = new ProductImportersChangeService(provider, notifier);

        await service.NotifyImportersChanged();

        provider.ReloadWasCalled.Should().BeTrue();
        notifier.NotifyWasCalled.Should().BeTrue();
    }

    private class FakeProductImporterProvider
        : IProductImporterProvider
    {
        public bool ReloadWasCalled { get; private set; }

        public List<IProductImporter> GetImporters()
        {
            return [];
        }

        public void Reload()
        {
            ReloadWasCalled = true;
        }
    }

    private class FakeProductImportersChangeNotifier
        : IProductImportersChangeNotifier
    {
        public bool NotifyWasCalled { get; private set; }

        public Task NotifyImportersChanged()
        {
            NotifyWasCalled = true;
            return Task.CompletedTask;
        }
    }
}
