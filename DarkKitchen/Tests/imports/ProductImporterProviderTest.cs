using BusinessLogics;
using FluentAssertions;

namespace Tests.imports;
[TestClass]
public class ProductImporterProviderTest
{
    [TestMethod]
    public void GetImporters_WhenPluginsFolderDoesNotExist_ShouldReturnEmptyList()
    {
        var pluginsPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var provider = new ProductImporterProvider(pluginsPath);

        var result = provider.GetImporters();

        result.Should().BeEmpty();
    }
}
