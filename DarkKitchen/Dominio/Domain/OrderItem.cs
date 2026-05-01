using Domain.Exceptions;

namespace Domain;

public class OrderItem
{
    private const int MinQuantity = 1;
    private const decimal MinUnitPrice = 0m;
    private const decimal MinDiscount = 0m;
    private const decimal MaxDiscount = 1m;

    private const string InvalidQuantityMessage = "La cantidad debe ser mayor a cero";
    private const string InvalidUnitPriceMessage = "El precio no puede ser negativo";
    private const string InvalidDiscountMessage = "El descuento debe estar entre 0 y 1";

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

    public static OrderItem MakeOrderItem(
        int productId,
        int quantity,
        decimal unitPrice,
        decimal discountApplied = MinDiscount)
    {
        ValidateQuantity(quantity);
        ValidateUnitPrice(unitPrice);
        ValidateDiscount(discountApplied);

        return new OrderItem
        {
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            DiscountApplied = discountApplied
        };
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity < MinQuantity)
        {
            throw new OrderException(InvalidQuantityMessage);
        }
    }

    private static void ValidateUnitPrice(decimal unitPrice)
    {
        if (unitPrice < MinUnitPrice)
        {
            throw new OrderException(InvalidUnitPriceMessage);
        }
    }

    private static void ValidateDiscount(decimal discountApplied)
    {
        if (discountApplied < MinDiscount || discountApplied > MaxDiscount)
        {
            throw new OrderException(InvalidDiscountMessage);
        }
    }
}
