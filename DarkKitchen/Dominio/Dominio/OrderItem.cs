using Dominio.Exceptions;

namespace Dominio;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public Product? Product { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountApplied { get; private set; }

    public decimal LineTotal => UnitPrice * Quantity * (1 - DiscountApplied);

    private OrderItem()
    {
    }

    public static OrderItem MakeOrderItem(int productId, int quantity, decimal unitPrice, decimal discountApplied = 0)
    {
        if(quantity <= 0)
        {
            throw new OrderException("La cantidad debe ser mayor a cero");
        }

        if(unitPrice < 0)
        {
            throw new OrderException("El precio no puede ser negativo");
        }

        if(discountApplied < 0 || discountApplied > 1)
        {
            throw new OrderException("El descuento debe estar entre 0 y 1");
        }

        return new OrderItem
        {
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            DiscountApplied = discountApplied
        };
    }
}
