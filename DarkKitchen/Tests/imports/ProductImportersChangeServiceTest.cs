using BusinessLogics;
using BusinessLogicsAbstractions;
using Moq;

namespace Tests.imports;

[TestClass]
public class ProductImportersChangeServiceTest
{
    [TestMethod]
    public async Task NotifyImportersChanged_ShouldNotify()
    {
        var notifierMock =
            new Mock<IProductImportersChangeNotifier>(MockBehavior.Strict);

        notifierMock
            .Setup(notifier => notifier.NotifyImportersChanged())
            .Returns(Task.CompletedTask);

        var service = new ProductImportersChangeService(
            notifierMock.Object);

        await service.NotifyImportersChanged();

        notifierMock.Verify(
            notifier => notifier.NotifyImportersChanged(),
            Times.Once);

        notifierMock.VerifyNoOtherCalls();
    }
}
