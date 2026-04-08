using Dominio;

namespace AlmacenamientoAbstracciones;

public interface IOrderRepository
{
    Order Add(Order order);
    Order? GetById(int id);
    List<Order> GetByClientId(int clientId, DateTime? dateFrom, DateTime? dateTo, OrderStatus? status);
    List<Order> GetByFilters(DateTime dateFrom, DateTime dateTo, string? street, OrderStatus? status);
    void Update(Order order);
}
