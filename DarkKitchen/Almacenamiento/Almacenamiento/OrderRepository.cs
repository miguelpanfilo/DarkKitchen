using AlmacenamientoAbstracciones;
using Dominio;

namespace Almacenamiento;

public class OrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = [];
    private int _lastId = 0;

    public Order Add(Order order)
    {
        _lastId++;
        order.Id = _lastId;
        _orders.Add(order);
        return order;
    }

    public Order? GetById(int id)
    {
        return _orders.FirstOrDefault(o => o.Id == id);
    }

    public List<Order> GetByClientId(int clientId, DateTime? dateFrom, DateTime? dateTo, OrderStatus? status)
    {
        var query = _orders.Where(o => o.ClientId == clientId);

        if(dateFrom.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= dateFrom.Value);
        }

        if(dateTo.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= dateTo.Value);
        }

        if(status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        return query.ToList();
    }

    public List<Order> GetByFilters(DateTime dateFrom, DateTime dateTo, string? street, OrderStatus? status)
    {
        var query = _orders.Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo);

        if(!string.IsNullOrWhiteSpace(street))
        {
            query = query.Where(o => o.Address.Street == street);
        }

        if(status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        return query.ToList();
    }

    public void Update(Order order)
    {
        var index = _orders.FindIndex(o => o.Id == order.Id);

        if(index >= 0)
        {
            _orders[index] = order;
        }
    }
}
