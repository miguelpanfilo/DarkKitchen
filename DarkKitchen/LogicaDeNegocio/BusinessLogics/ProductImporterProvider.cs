using System.Reflection;
using BusinessLogicsAbstractions;

namespace BusinessLogics;

public class ProductImporterProvider(string pluginsPath,
    IPluginAssemblyLoader assemblyLoader) : IProductImporterProvider
{
    public List<IProductImporter> GetImporters()
    {
        Directory.CreateDirectory(pluginsPath);

        return Directory
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
}
