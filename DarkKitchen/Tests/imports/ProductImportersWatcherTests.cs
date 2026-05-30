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
}
