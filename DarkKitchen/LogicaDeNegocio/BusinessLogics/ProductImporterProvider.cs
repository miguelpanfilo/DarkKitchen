using System.Reflection;
using BusinessLogicsAbstractions.Imports;

namespace BusinessLogics;

public class ProductImporterProvider(string pluginsPath) : IProductImporterProvider
{
    public List<IProductImporter> GetImporters()
    {
        Directory.CreateDirectory(pluginsPath);

        return Directory
            .GetFiles(pluginsPath, "*.dll")
            .SelectMany(LoadImportersFromAssembly)
            .ToList();
    }

    private static IEnumerable<IProductImporter> LoadImportersFromAssembly(string dllPath)
    {
        var assembly = Assembly.LoadFrom(dllPath);

        return assembly
            .GetTypes()
            .Where(type =>
                typeof(IProductImporter).IsAssignableFrom(type)
                && !type.IsInterface
                && !type.IsAbstract)
            .Select(type => (IProductImporter)Activator.CreateInstance(type)!);
    }
}
