using BusinessLogics;
using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Products;
using BusinessLogicsAbstractions.Promotions;
using Contract.Orders;
using Contract.Products;
using FluentAssertions;
using Moq;

namespace Tests.Orders;

[TestClass]
public sealed class OrderItemResolverTests
{
    private Mock<IProductService> _productServiceMock = null!;
    private Mock<IPromotionService> _promotionServiceMock = null!;
    private OrderItemResolver _resolver = null!;

    [TestInitialize]
    public void Initialize()
    {
        _productServiceMock = new Mock<IProductService>(MockBehavior.Strict);
        _promotionServiceMock = new Mock<IPromotionService>(MockBehavior.Strict);

        _resolver = new OrderItemResolver(
            _productServiceMock.Object,
            _promotionServiceMock.Object);
    }

    private static ProductResponseDto MakeActiveProduct(decimal price = 100m)
    {
        return new ProductResponseDto
        {
            Code = "COD123",
            Name = "Test Product",
            Description = "Descripción de prueba válida",
            CommercialLine = "Línea",
            Category = "Categoría",
            Price = price,
            IsActive = true,
            Photos = [new PhotoResponseDto { Path = "foto0.jpg" }]
        };
    }

    [TestMethod]
    public void Resolve_WhenProductIsInactive_ShouldThrowValidationException()
    {
        var product = new ProductResponseDto
        {
            Code = "COD123",
            Name = "Producto Test",
            Description = "Descripción de prueba válida",
            CommercialLine = "Línea",
            Category = "Categoría",
            Price = 100m,
            IsActive = false,
            Photos = [new PhotoResponseDto { Path = "foto0.jpg" }]
        };

        _productServiceMock
            .Setup(p => p.GetById(1))
            .Returns(product);

        var act = () => _resolver.Resolve(
        [
            new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }
        ]);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("El producto Producto Test no está disponible");

        _productServiceMock.VerifyAll();
        _promotionServiceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void Resolve_WhenProductHasCurrentPromotion_ShouldApplyHighestDiscount()
    {
        _productServiceMock
            .Setup(p => p.GetById(1))
            .Returns(MakeActiveProduct(price: 520m));

        _promotionServiceMock
            .Setup(p => p.GetHighestCurrentDiscountForProduct(1))
            .Returns(0.20m);

        var result = _resolver.Resolve(
        [
            new CreateOrderItemRequest { ProductId = 1, Quantity = 1 }
        ]);

        result.Should().ContainSingle();
        result[0].ProductId.Should().Be(1);
        result[0].Quantity.Should().Be(1);
        result[0].UnitPrice.Should().Be(520m);
        result[0].DiscountApplied.Should().Be(0.20m);

        _productServiceMock.VerifyAll();
        _promotionServiceMock.VerifyAll();
    }

    [TestMethod]
    public void Resolve_WhenProductHasNoCurrentPromotion_ShouldApplyZeroDiscount()
    {
        _productServiceMock
            .Setup(p => p.GetById(1))
            .Returns(MakeActiveProduct(price: 520m));

        _promotionServiceMock
            .Setup(p => p.GetHighestCurrentDiscountForProduct(1))
            .Returns(0m);

        var result = _resolver.Resolve(
        [
            new CreateOrderItemRequest { ProductId = 1, Quantity = 1 }
        ]);

        result.Should().ContainSingle();
        result[0].ProductId.Should().Be(1);
        result[0].Quantity.Should().Be(1);
        result[0].UnitPrice.Should().Be(520m);
        result[0].DiscountApplied.Should().Be(0m);

        _productServiceMock.VerifyAll();
        _promotionServiceMock.VerifyAll();
    }
}
