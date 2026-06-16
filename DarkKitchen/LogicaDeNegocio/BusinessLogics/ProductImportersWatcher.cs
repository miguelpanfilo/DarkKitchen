using BusinessLogicsAbstractions;

namespace BusinessLogics;

public class ProductImportersWatcher : IDisposable
{
    private readonly IProductImportersChangeNotifier notifier;
    private readonly string pluginsPath;
    private FileSystemWatcher? watcher;
    private Timer? pollingTimer;
    private HashSet<string> lastSnapshot = new();

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

        lastSnapshot = GetCurrentSnapshot();

        watcher = new FileSystemWatcher(pluginsPath, "*.dll");
        watcher.Created += OnPluginChanged;
        watcher.Deleted += OnPluginChanged;
        watcher.Renamed += OnPluginChanged;
        watcher.EnableRaisingEvents = true;

        pollingTimer = new Timer(PollForChanges, null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
    }

    private async void OnPluginChanged(object? sender, FileSystemEventArgs e)
    {
        lastSnapshot = GetCurrentSnapshot();
        await notifier.NotifyImportersChanged();
    }

    private async void PollForChanges(object? state)
    {
        var current = GetCurrentSnapshot();
        if (!current.SetEquals(lastSnapshot))
        {
            lastSnapshot = current;
            await notifier.NotifyImportersChanged();
        }
    }

    private HashSet<string> GetCurrentSnapshot()
    {
        if (!Directory.Exists(pluginsPath))
        {
            return new HashSet<string>();
        }

        return Directory.GetFiles(pluginsPath, "*.dll")
            .Select(Path.GetFileName)
            .OfType<string>()
            .ToHashSet();
    }

    public void Dispose()
    {
        pollingTimer?.Dispose();
        watcher?.Dispose();
    }
}
