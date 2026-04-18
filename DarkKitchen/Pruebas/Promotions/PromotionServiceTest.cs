using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Builders;
using Dominio.Exceptions;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.Promotions;

[TestClass]
public class PromotionServiceTest
{
    private Product CreateValidProduct()
    {
        return new ProductBuilder()
            .WithCode("COD123")
            .WithName("Nombre válido producto")
            .WithDescription("Descripción válida con más de veinte caracteres")
            .WithCommercialLine("Línea")
            .WithCategory("Categoría")
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto> { new ProductPhoto("photo-1.jpg") })
            .Build();
    }

    [TestMethod]
    public void GivenValidPromotionDto_WhenCreating_ThenRepositoryIsCalledAndPromotionIsReturned()
    {
        var promotionDto = new PromotionDto(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var repositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        repositoryMock
            .Setup(r => r.Create(It.IsAny<Promotion>()))
            .Returns((Promotion promotion) => promotion);

        var service = new PromotionService(repositoryMock.Object, productRepositoryMock.Object);

        var result = service.Create(promotionDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(promotionDto.Name, result.Name);
        Assert.AreEqual(promotionDto.DiscountPercentage, result.DiscountPercentage);
        Assert.AreEqual(promotionDto.StartDate, result.StartDate);
        Assert.AreEqual(promotionDto.EndDate, result.EndDate);

        repositoryMock.Verify(r => r.Create(It.IsAny<Promotion>()), Times.Once);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void GivenPromotionDtoWithInvalidName_WhenCreating_ThenThrowsPromotionException(string invalidName)
    {
        var promotionDto = new PromotionDto(
            invalidName,
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var repositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var service = new PromotionService(repositoryMock.Object, productRepositoryMock.Object);

        var ex = Assert.ThrowsException<PromotionException>(() => service.Create(promotionDto));

        Assert.AreEqual("El nombre de la promoción no puede ser vacío", ex.Message);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(101)]
    public void GivenPromotionDtoWithInvalidDiscountPercentage_WhenCreating_ThenThrowsPromotionException(
        int invalidDiscount)
    {
        var promotionDto = new PromotionDto(
            "Black Friday",
            (decimal)invalidDiscount,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var repositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var service = new PromotionService(repositoryMock.Object, productRepositoryMock.Object);

        var ex = Assert.ThrowsException<PromotionException>(() => service.Create(promotionDto));

        Assert.AreEqual("El porcentaje de descuento debe estar entre 1 y 100", ex.Message);
    }

    [DataTestMethod]
    [DataRow(2026, 1, 30, 2026, 1, 25)]
    public void GivenPromotionDtoWithInvalidDateRange_WhenCreating_ThenThrowsPromotionException(
        int startYear,
        int startMonth,
        int startDay,
        int endYear,
        int endMonth,
        int endDay)
    {
        var promotionDto = new PromotionDto(
            "Black Friday",
            10,
            new DateOnly(startYear, startMonth, startDay),
            new DateOnly(endYear, endMonth, endDay));

        var repositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var service = new PromotionService(repositoryMock.Object, productRepositoryMock.Object);

        var ex = Assert.ThrowsException<PromotionException>(() => service.Create(promotionDto));

        Assert.AreEqual("La fecha de inicio no puede ser mayor a la fecha de fin", ex.Message);
    }

    [TestMethod]
    public void GivenExistingPromotionAndProduct_WhenAssociatingProduct_ThenProductIsAddedToPromotion()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var product = CreateValidProduct();

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        productRepositoryMock
            .Setup(r => r.GetById(100))
            .Returns(product);

        promotionRepositoryMock
            .Setup(r => r.Update(promotion))
            .Returns(promotion);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        service.AssociateProduct(1, 100);

        Assert.IsTrue(promotion.Products.Contains(product));

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenNonExistingPromotion_WhenAssociatingProduct_ThenThrowsPromotionException()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns((Promotion)null!);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var ex = Assert.ThrowsException<PromotionException>(() => service.AssociateProduct(1, 100)
        );

        Assert.AreEqual("La promoción no existe", ex.Message);

        productRepositoryMock.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotionAndNonExistingProduct_WhenAssociatingProduct_ThenThrowsPromotionException()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        productRepositoryMock
            .Setup(r => r.GetById(100))
            .Returns((Product)null!);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var ex = Assert.ThrowsException<PromotionException>(() => service.AssociateProduct(1, 100)
        );

        Assert.AreEqual("El producto no existe", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenAssociatedProduct_WhenDisassociatingFromPromotion_ThenProductIsRemoved()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30));
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var product = CreateValidProduct();

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        productRepositoryMock
            .Setup(r => r.GetById(100))
            .Returns(product);

        promotionRepositoryMock
            .Setup(r => r.Update(promotion))
            .Returns(promotion);

        var servicePromotion = new PromotionService(promotionRepositoryMock.Object, productRepositoryMock.Object);

        servicePromotion.AssociateProduct(1, 100);

        servicePromotion.DisassociateProduct(1, 100);

        Assert.IsFalse(promotion.Products.Contains(product));

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenNonExistingPromotion_WhenDisassociatingProduct_ThenThrowsPromotionException()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns((Promotion)null!);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var ex = Assert.ThrowsException<PromotionException>(() => service.DisassociateProduct(1, 100)
        );

        Assert.AreEqual("La promoción no existe", ex.Message);

        productRepositoryMock.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotionAndNonExistingProduct_WhenDisassociatingProduct_ThenThrowsPromotionException()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        productRepositoryMock
            .Setup(r => r.GetById(100))
            .Returns((Product)null!);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var ex = Assert.ThrowsException<PromotionException>(() => service.DisassociateProduct(1, 100)
        );

        Assert.AreEqual("El producto no existe", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenPatchingName_ThenPromotionNameIsUpdated()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var patchDto = new PromotionPatchDto
        {
            Name = "Cyber Monday"
        };

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        promotionRepositoryMock
            .Setup(r => r.Update(promotion))
            .Returns(promotion);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("Cyber Monday", result.Name);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithEmptyName_ThenThrowsPromotionException()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var patchDto = new PromotionPatchDto
        {
            Name = string.Empty
        };

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var ex = Assert.ThrowsException<PromotionException>(() => service.Update(1, patchDto));

        Assert.AreEqual("El nombre de la promoción no puede ser vacío", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithValidDiscountPercentage_ThenPromotionDiscountPercentageIsUpdated()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var patchDto = new PromotionPatchDto
        {
            DiscountPercentage = 25
        };

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        promotionRepositoryMock
            .Setup(r => r.Update(promotion))
            .Returns(promotion);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(25, result.DiscountPercentage);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(101)]
    public void GivenExistingPromotion_WhenUpdatingWithInvalidDiscountPercentage_ThenThrowsPromotionException(
        int invalidDiscount)
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var patchDto = new PromotionPatchDto
        {
            DiscountPercentage = invalidDiscount
        };

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var ex = Assert.ThrowsException<PromotionException>(() => service.Update(1, patchDto));

        Assert.AreEqual("El porcentaje de descuento debe estar entre 1 y 100", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithValidStartDate_ThenPromotionStartDateIsUpdated()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var newStartDate = new DateOnly(2026, 1, 26);

        var patchDto = new PromotionPatchDto
        {
            StartDate = newStartDate
        };

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        promotionRepositoryMock
            .Setup(r => r.Update(promotion))
            .Returns(promotion);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(newStartDate, result.StartDate);
        Assert.AreEqual(new DateOnly(2026, 1, 30), result.EndDate);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithValidEndDate_ThenPromotionEndDateIsUpdated()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var newEndDate = new DateOnly(2026, 1, 31);

        var patchDto = new PromotionPatchDto
        {
            EndDate = newEndDate
        };

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        promotionRepositoryMock
            .Setup(r => r.Update(promotion))
            .Returns(promotion);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(new DateOnly(2026, 1, 25), result.StartDate);
        Assert.AreEqual(newEndDate, result.EndDate);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }
}
