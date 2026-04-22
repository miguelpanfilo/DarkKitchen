using AlmacenamientoAbstracciones;
using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Almacenamiento.Repositories;

public class OrderRepository(SqlContext context) : IOrderRepository
{
    private readonly SqlContext _context = context;

    public Order Add(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
        return order;
    }

    public Order? GetById(int id)
    {
        return _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p!.Photos)
            .Include(o => o.Client)
            .FirstOrDefault(o => o.Id == id);
    }

    public List<Order> GetByClientId(int clientId, DateTime? dateFrom, DateTime? dateTo, OrderStatus? status)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Client)
            .Where(o => o.ClientId == clientId);

        if (dateFrom.HasValue) query = query.Where(o => o.CreatedAt >= dateFrom.Value);
        if (dateTo.HasValue)   query = query.Where(o => o.CreatedAt <= dateTo.Value);
        if (status.HasValue)   query = query.Where(o => o.Status == status.Value);

        return query.ToList();
    }

    public List<Order> GetByFilters(DateTime dateFrom, DateTime dateTo, string? street, OrderStatus? status)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Client)
            .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo);

        if (!string.IsNullOrWhiteSpace(street))
            query = query.Where(o => o.Address.Street == street);
        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        return query.ToList();
    }

    public void Update(Order order)
    {
        _context.Orders.Update(order);
        _context.SaveChanges();
    }
}
