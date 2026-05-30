using BusinessLogicsAbstractions.Imports;

namespace BusinessLogics;

public class ProductImportersChangeService(
    IProductImportersChangeNotifier notifier)
    : IProductImportersChangeService
{
    public Task NotifyImportersChanged()
    {
        return notifier.NotifyImportersChanged();
    }
}
