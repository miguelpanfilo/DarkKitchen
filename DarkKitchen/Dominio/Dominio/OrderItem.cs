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

    private OrderItem() { }

    public static OrderItem MakeOrderItem(int productId, int quantity, decimal unitPrice, decimal discountApplied = 0)
    {
        if(quantity <= 0)
        {
            throw new OrderException("Quantity must be greater than zero");
        }

        if(unitPrice < 0)
        {
            throw new OrderException("Unit price cannot be negative");
        }

        if(discountApplied < 0 || discountApplied > 1)
        {
            throw new OrderException("Discount must be between 0 and 1");
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
