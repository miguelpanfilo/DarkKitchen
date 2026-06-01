using BusinessLogicsAbstractions;

namespace BusinessLogics;

public class ProductImportersWatcher : IDisposable
{
    private readonly IProductImportersChangeNotifier notifier;
    private readonly string pluginsPath;
    private FileSystemWatcher? watcher;

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

        watcher = new FileSystemWatcher(
            pluginsPath,
            "*.dll");

        watcher.Created += OnPluginChanged;
        watcher.Deleted += OnPluginChanged;
        watcher.Renamed += OnPluginChanged;

        watcher.EnableRaisingEvents = true;
    }

    private async void OnPluginChanged(
        object? sender,
        FileSystemEventArgs e)
    {
        await notifier.NotifyImportersChanged();
    }

    public void Dispose()
    {
        watcher?.Dispose();
    }
}
