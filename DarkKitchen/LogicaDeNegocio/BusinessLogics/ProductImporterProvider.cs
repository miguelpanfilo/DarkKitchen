using System.Reflection;
using BusinessLogicsAbstractions;

namespace BusinessLogics;

public class ProductImporterProvider(string pluginsPath,
    IPluginAssemblyLoader assemblyLoader) : IProductImporterProvider
{
    private List<IProductImporter> _importers = [];

    public List<IProductImporter> GetImporters()
    {
        Directory.CreateDirectory(pluginsPath);
        return _importers;
    }

    public void Reload()
    {
        Directory.CreateDirectory(pluginsPath);
        ClearShadowFolder();

        _importers = Directory
            .GetFiles(pluginsPath, "*.dll")
            .SelectMany(LoadImportersFromAssembly)
            .ToList();
    }

    private IEnumerable<IProductImporter> LoadImportersFromAssembly(string dllPath)
    {
        try
        {
            var assembly = assemblyLoader.Load(dllPath);

            return assembly
                .GetTypes()
                .Where(type =>
                    typeof(IProductImporter).IsAssignableFrom(type)
                    && !type.IsInterface
                    && !type.IsAbstract)
                .Select(type => (IProductImporter)Activator.CreateInstance(type)!);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private void ClearShadowFolder()
    {
        var shadowFolder = Path.Combine(pluginsPath, "plugins_shadow");

        if(Directory.Exists(shadowFolder))
        {
            Directory.Delete(shadowFolder, recursive: true);
        }
    }
}
