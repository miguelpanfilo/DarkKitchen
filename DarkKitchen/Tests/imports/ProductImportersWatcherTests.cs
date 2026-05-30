using BusinessLogics;
using BusinessLogicsAbstractions.Imports;
using FluentAssertions;
using Moq;

namespace Tests.imports;

[TestClass]
public class ProductImportersWatcherTests
{
    [TestMethod]
    public void Start_WhenPluginsFolderDoesNotExist_ShouldCreatePluginsFolder()
    {
        var notifierMock = new Mock<IProductImportersChangeNotifier>(MockBehavior.Strict);

        var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var watcher = new ProductImportersWatcher(
            notifierMock.Object,
            folderPath);

        watcher.Start();

        Directory.Exists(folderPath).Should().BeTrue();

        notifierMock.VerifyNoOtherCalls();

        watcher.Dispose();
    }

    [TestMethod]
    public async Task Start_WhenDllIsAdded_ShouldNotifyImportersChanged()
    {
        var notifierMock = new Mock<IProductImportersChangeNotifier>(
            MockBehavior.Strict);

        notifierMock
            .Setup(notifier => notifier.NotifyImportersChanged())
            .Returns(Task.CompletedTask);

        var folderPath = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString());

        var watcher = new ProductImportersWatcher(
            notifierMock.Object,
            folderPath);

        watcher.Start();

        var dllPath = Path.Combine(folderPath, "importador.dll");

        await File.WriteAllTextAsync(dllPath, "contenido fake");

        await Task.Delay(500);

        notifierMock.Verify(
            notifier => notifier.NotifyImportersChanged(),
            Times.AtLeastOnce);

        notifierMock.VerifyNoOtherCalls();

        watcher.Dispose();
    }

    [TestMethod]
    public async Task Start_WhenDllIsDeleted_ShouldNotifyImportersChanged()
    {
        var notifierMock = new Mock<IProductImportersChangeNotifier>(
            MockBehavior.Strict);

        notifierMock
            .Setup(notifier => notifier.NotifyImportersChanged())
            .Returns(Task.CompletedTask);

        var folderPath = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(folderPath);

        var dllPath = Path.Combine(
            folderPath,
            "importador.dll");

        await File.WriteAllTextAsync(
            dllPath,
            "contenido fake");

        using var watcher = new ProductImportersWatcher(
            notifierMock.Object,
            folderPath);

        watcher.Start();

        File.Delete(dllPath);

        await Task.Delay(500);

        notifierMock.Verify(
            notifier => notifier.NotifyImportersChanged(),
            Times.AtLeastOnce);

        notifierMock.VerifyNoOtherCalls();
    }
}
