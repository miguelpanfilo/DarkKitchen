using DarkKitchen.Hubs;
using DarkKitchen.SignalR;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Tests.WebApi;

[TestClass]
public class ProductImportersSignalRNotifierTest
{
    [TestMethod]
    public async Task NotifyImportersChanged_ShouldSendSignalREvent()
    {
        var hubContextMock =
            new Mock<IHubContext<ProductImportersHub>>(MockBehavior.Strict);

        var clientsMock =
            new Mock<IHubClients>(MockBehavior.Strict);

        var clientProxyMock =
            new Mock<IClientProxy>(MockBehavior.Strict);

        hubContextMock
            .Setup(hubContext => hubContext.Clients)
            .Returns(clientsMock.Object);

        clientsMock
            .Setup(clients => clients.All)
            .Returns(clientProxyMock.Object);

        clientProxyMock
            .Setup(clientProxy => clientProxy.SendCoreAsync(
                "ImportersChanged",
                It.Is<object[]>(args => args.Length == 0),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var notifier = new ProductImportersSignalRNotifier(
            hubContextMock.Object);

        await notifier.NotifyImportersChanged();

        clientProxyMock.VerifyAll();
        clientsMock.VerifyAll();
        hubContextMock.VerifyAll();

        clientProxyMock.VerifyNoOtherCalls();
        clientsMock.VerifyNoOtherCalls();
        hubContextMock.VerifyNoOtherCalls();
    }
}
