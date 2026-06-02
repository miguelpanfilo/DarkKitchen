using System.Reflection;
using BusinessLogicsAbstractions;

namespace BusinessLogics;

public class ProductImporterProvider(string pluginsPath,
    IPluginAssemblyLoader assemblyLoader) : IProductImporterProvider
{
    private readonly List<IProductImporter> _importers = [];

    public List<IProductImporter> GetImporters()
    {
        Directory.CreateDirectory(pluginsPath);
        return _importers;
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
}
