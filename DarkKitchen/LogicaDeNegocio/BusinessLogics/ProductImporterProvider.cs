using BusinessLogicsAbstractions.Imports;

namespace BusinessLogics;

public class ProductImporterProvider(string pluginsPath) : IProductImporterProvider
{
    public List<IProductImporter> GetImporters()
    {
        if (!Directory.Exists(pluginsPath))
        {
            return [];
        }

        return [];
    }
}
