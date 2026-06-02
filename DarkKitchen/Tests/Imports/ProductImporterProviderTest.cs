using System.Reflection;
using BusinessLogics;
using BusinessLogicsAbstractions;
using Contract;
using FluentAssertions;

namespace Tests.imports;
[TestClass]
public class ProductImporterProviderTest
{
    [TestMethod]
    public void GetImporters_WhenDllExists_ShouldUseAssemblyLoader()
    {
        var pluginsPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(pluginsPath);

        var dllPath = Path.Combine(pluginsPath, "TestProductImporter.dll");
        File.WriteAllText(dllPath, string.Empty);

        var loader = new FakePluginAssemblyLoader(typeof(TestProductImporter).Assembly);

        var provider = new ProductImporterProvider(pluginsPath, loader);

        var result = provider.GetImporters();

        loader.LoadedPaths
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(dllPath);
        result.Should().ContainSingle();
    }

    [TestMethod]
    public void GetImporters_WhenPluginsFolderDoesNotExist_ShouldReturnEmptyList()
    {
        var pluginsPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var provider = new ProductImporterProvider(
            pluginsPath,
            new PluginAssemblyLoader());

        var result = provider.GetImporters();

        result.Should().BeEmpty();
    }

    [TestMethod]
    public void GetImporters_WhenPluginsFolderExistsAndHasNoDlls_ShouldReturnEmptyList()
    {
        var pluginsPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Directory.CreateDirectory(pluginsPath);

        var provider = new ProductImporterProvider(
            pluginsPath,
            new PluginAssemblyLoader());

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

        var provider = new ProductImporterProvider(
            pluginsPath,
            new PluginAssemblyLoader());

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

    private class FakePluginAssemblyLoader(Assembly assembly) : IPluginAssemblyLoader
    {
        public List<string> LoadedPaths { get; } = [];

        public Assembly Load(string dllPath)
        {
            LoadedPaths.Add(dllPath);
            return assembly;
        }
    }
}
