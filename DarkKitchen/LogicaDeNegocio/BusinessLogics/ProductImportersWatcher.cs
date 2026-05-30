using BusinessLogicsAbstractions.Imports;

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

        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;

        watcher.EnableRaisingEvents = true;
    }

    private async void OnCreated(
        object? sender,
        FileSystemEventArgs e)
    {
        await notifier.NotifyImportersChanged();
    }

    private async void OnDeleted(
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
