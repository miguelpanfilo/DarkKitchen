using BusinessLogicsAbstractions;
using DarkKitchen.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DarkKitchen.SignalR;

public class ProductImportersSignalRNotifier(
    IHubContext<ProductImportersHub> hubContext)
    : IProductImportersChangeNotifier
{
    public Task NotifyImportersChanged()
    {
        return hubContext.Clients.All.SendAsync("ImportersChanged");
    }
}
