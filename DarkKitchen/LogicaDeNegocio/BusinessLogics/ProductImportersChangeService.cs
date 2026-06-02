using BusinessLogicsAbstractions;

namespace BusinessLogics;

public class ProductImportersChangeService(
    IProductImporterProvider importerProvider,
    IProductImportersChangeNotifier notifier)
    : IProductImportersChangeService
{
    public Task NotifyImportersChanged()
    {
        importerProvider.Reload();

        return notifier.NotifyImportersChanged();
    }
}
