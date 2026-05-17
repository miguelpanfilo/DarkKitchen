using BusinessLogics;
using BusinessLogicsAbstractions.Imports;
using Contract.Importers;
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

    [TestMethod]
    public void GetImporters_WhenPluginsFolderExistsAndHasNoDlls_ShouldReturnEmptyList()
    {
        var pluginsPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Directory.CreateDirectory(pluginsPath);

        var provider = new ProductImporterProvider(pluginsPath);

        var result = provider.GetImporters();

        result.Should().BeEmpty();
    }

    [TestMethod]
    public void GetImporters_WhenDllContainsProductImporter_ShouldLoadImporter()
    {
        var pluginsPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(pluginsPath);

        var sourceDll = typeof(TestProductImporter).Assembly.Location;
        var targetDll = Path.Combine(pluginsPath, "TestProductImporter.dll");

        File.Copy(sourceDll, targetDll);

        var provider = new ProductImporterProvider(pluginsPath);

        var result = provider.GetImporters();

        result.Should().ContainSingle();
        result[0].Id.Should().Be("TestProductImporter");
        result[0].Name.Should().Be("Importador de prueba");
    }

    private class TestProductImporter : IProductImporter
    {
        public string Id => "TestProductImporter";

        public string Name => "Importador de prueba";
        public List<ImportedProductDto> Import(Stream fileStream)
        {
            return [];
        }
    }
}
