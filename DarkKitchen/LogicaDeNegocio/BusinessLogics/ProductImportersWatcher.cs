using BusinessLogicsAbstractions.Imports;

namespace BusinessLogics;

public class ProductImportersWatcher : IDisposable
{
    private readonly IProductImportersChangeNotifier notifier;
    private readonly string pluginsPath;

    public ProductImportersWatcher(
        IProductImportersChangeNotifier notifier,
        string pluginsPath)
    {
        this.notifier = notifier;
        this.pluginsPath = pluginsPath;
    }

    public void Start()
    {
        Directory.CreateDirectory(pluginsPath);
    }

    public void Dispose()
    {
    }
}
