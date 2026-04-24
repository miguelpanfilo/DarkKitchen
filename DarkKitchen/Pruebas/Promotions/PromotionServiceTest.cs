using AlmacenamientoAbstracciones;
using Contratos.Promotions;
using Dominio;
using Dominio.Builders;
using Dominio.Exceptions;
using LogicaDeNegocio.Promotions;
using LogicaDeNegocioAbstracciones.Exceptions;
using Moq;

namespace Pruebas.Promotions;

[TestClass]
public class PromotionServiceTest
{
    private DateOnly Today() =>
        DateOnly.FromDateTime(DateTime.UtcNow);

    private (DateOnly start, DateOnly end) FutureDates() =>
        (Today().AddDays(5), Today().AddDays(10));

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
        var promotionDto = new PromotionRequestDto
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            StartDate = new DateOnly(2026, 1, 25),
            EndDate = new DateOnly(2026, 1, 30)
        };

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
        var promotionDto = new PromotionRequestDto
        {
            Name = invalidName,
            DiscountPercentage = 10,
            StartDate = new DateOnly(2026, 1, 25),
            EndDate = new DateOnly(2026, 1, 30)
        };

        var repositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var service = new PromotionService(repositoryMock.Object, productRepositoryMock.Object);

        var ex = Assert.ThrowsException<ValidationException>(() => service.Create(promotionDto));

        Assert.AreEqual("El nombre de la promoción no puede ser vacío", ex.Message);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(101)]
    public void GivenPromotionDtoWithInvalidDiscountPercentage_WhenCreating_ThenThrowsPromotionException(
        int invalidDiscount)
    {
        var promotionDto = new PromotionRequestDto
        {
            Name = "Black Friday",
            DiscountPercentage = (decimal)invalidDiscount,
            StartDate = new DateOnly(2026, 1, 25),
            EndDate = new DateOnly(2026, 1, 30)
        };

        var repositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var service = new PromotionService(repositoryMock.Object, productRepositoryMock.Object);

        var ex = Assert.ThrowsException<ValidationException>(() => service.Create(promotionDto));

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
        var promotionDto = new PromotionRequestDto
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            StartDate = new DateOnly(startYear, startMonth, startDay),
            EndDate = new DateOnly(endYear, endMonth, endDay)
        };

        var repositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var service = new PromotionService(repositoryMock.Object, productRepositoryMock.Object);

        var ex = Assert.ThrowsException<ValidationException>(() => service.Create(promotionDto));

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

        Assert.IsTrue(promotion.PromotionProducts.Any(pp => pp.ProductId == 100));

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

        var ex = Assert.ThrowsException<NotFoundException>(() => service.AssociateProduct(1, 100)
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

        var ex = Assert.ThrowsException<NotFoundException>(() => service.AssociateProduct(1, 100)
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

        Assert.IsFalse(promotion.PromotionProducts.Any(pp => pp.ProductId == 100));

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

        var ex = Assert.ThrowsException<NotFoundException>(() => service.DisassociateProduct(1, 100)
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

        var ex = Assert.ThrowsException<NotFoundException>(() => service.DisassociateProduct(1, 100)
        );

        Assert.AreEqual("El producto no existe", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenPatchingName_ThenPromotionNameIsUpdated()
    {
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            start,
            end);

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
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            start,
            end);

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

        var ex = Assert.ThrowsException<ValidationException>(() => service.Update(1, patchDto));

        Assert.AreEqual("El nombre de la promoción no puede ser vacío", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithValidDiscountPercentage_ThenPromotionDiscountPercentageIsUpdated()
    {
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            start,
            end
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
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            start,
            end
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

        var ex = Assert.ThrowsException<ValidationException>(() => service.Update(1, patchDto));

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

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithValidStartDateAndEndDate_ThenPromotionDatesAreUpdated()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var newStartDate = new DateOnly(2026, 1, 26);
        var newEndDate = new DateOnly(2026, 1, 31);

        var patchDto = new PromotionPatchDto
        {
            StartDate = newStartDate,
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
        Assert.AreEqual(newStartDate, result.StartDate);
        Assert.AreEqual(newEndDate, result.EndDate);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithInvalidStartDate_ThenThrowsPromotionException()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var patchDto = new PromotionPatchDto
        {
            StartDate = new DateOnly(2026, 1, 31)
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

        var ex = Assert.ThrowsException<ValidationException>(() => service.Update(1, patchDto));

        Assert.AreEqual("La fecha de inicio no puede ser mayor a la fecha de fin", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithInvalidEndDate_ThenThrowsPromotionException()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var patchDto = new PromotionPatchDto
        {
            EndDate = new DateOnly(2026, 1, 24)
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

        var ex = Assert.ThrowsException<ValidationException>(() => service.Update(1, patchDto));

        Assert.AreEqual("La fecha de inicio no puede ser mayor a la fecha de fin", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithInvalidDateRange_ThenThrowsPromotionException()
    {
        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var patchDto = new PromotionPatchDto
        {
            StartDate = new DateOnly(2026, 2, 5),
            EndDate = new DateOnly(2026, 1, 31)
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

        var ex = Assert.ThrowsException<ValidationException>(() => service.Update(1, patchDto));

        Assert.AreEqual("La fecha de inicio no puede ser mayor a la fecha de fin", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenNonExistingPromotion_WhenUpdating_ThenThrowsPromotionException()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var patchDto = new PromotionPatchDto
        {
            Name = "Cyber Monday"
        };

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns((Promotion)null!);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var ex = Assert.ThrowsException<NotFoundException>(() => service.Update(1, patchDto)
        );

        Assert.AreEqual("La promoción no existe", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenDeleting_ThenMarksAsDeletedAndUpdatesPromotion()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        promotionRepositoryMock
            .Setup(r => r.Update(It.Is<Promotion>(p => p.IsDeleted)))
            .Returns(promotion);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        service.Delete(1);

        promotionRepositoryMock.Verify(r => r.GetById(1), Times.Once);
        promotionRepositoryMock.Verify(r => r.Update(It.Is<Promotion>(
            p => p.IsDeleted)), Times.Once);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenNonExistingPromotion_WhenDeleting_ThenThrowsPromotionException()
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

        var ex = Assert.ThrowsException<NotFoundException>(() => service.Delete(1));

        Assert.AreEqual("La promoción no existe", ex.Message);

        promotionRepositoryMock.Verify(r => r.GetById(1), Times.Once);
        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenDeletedPromotion_WhenDeleting_ThenThrowsPromotionException()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        promotion.Delete();

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var exception = Assert.ThrowsException<NotFoundException>(() => service.Delete(1));

        Assert.AreEqual("La promoción no existe", exception.Message);

        promotionRepositoryMock.Verify(r => r.GetById(1), Times.Once);
        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenDeletedPromotion_WhenUpdating_ThenThrowsPromotionException()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );
        promotion.Delete();

        var patchDto = new PromotionPatchDto
        {
            Name = "Cyber Monday"
        };

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var ex = Assert.ThrowsException<NotFoundException>(() =>
            service.Update(1, patchDto)
        );

        Assert.AreEqual("La promoción no existe", ex.Message);

        promotionRepositoryMock.Verify(r => r.GetById(1), Times.Once);
        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenMissingDate_WhenGettingPromotions_ThenThrowsValidationException()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var request = new PromotionSearchRequestDto
        {
            Date = null
        };

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var ex = Assert.ThrowsException<ValidationException>(() => service.GetPromotions(request));

        Assert.AreEqual("La fecha es obligatoria", ex.Message);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenValidDate_WhenGettingPromotions_ThenReturnsOnlyActivePromotions()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var date = new DateOnly(2026, 1, 26);

        var activePromotion = Promotion.MakePromotion(
            "Promo Activa",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );

        var expiredPromotion = Promotion.MakePromotion(
            "Promo Vencida",
            10,
            new DateOnly(2026, 1, 10),
            new DateOnly(2026, 1, 20)
        );

        promotionRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(new List<Promotion> { activePromotion, expiredPromotion });

        var request = new PromotionSearchRequestDto
        {
            Date = date
        };

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var result = service.GetPromotions(request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Promo Activa", result[0].Name);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenValidDateAndProductId_WhenGettingPromotions_ThenReturnsOnlyPromotionsForThatProduct()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var date = new DateOnly(2026, 1, 26);

        var matchingPromotion = Promotion.MakePromotion(
            "Promo Producto 100",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );
        matchingPromotion.AssociateProduct(100);

        var otherPromotion = Promotion.MakePromotion(
            "Promo Otro Producto",
            15,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );
        otherPromotion.AssociateProduct(200);

        var product100 = new ProductBuilder()
            .WithCode("COD100")
            .WithName("Producto 100")
            .WithDescription("Descripción válida con más de veinte caracteres")
            .WithCommercialLine("Línea A")
            .WithCategory("Categoría")
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto> { new ProductPhoto("photo-1.jpg") })
            .Build();

        var product200 = new ProductBuilder()
            .WithCode("COD200")
            .WithName("Producto 200")
            .WithDescription("Descripción válida con más de veinte caracteres")
            .WithCommercialLine("Línea B")
            .WithCategory("Categoría")
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto> { new ProductPhoto("photo-2.jpg") })
            .Build();

        promotionRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(new List<Promotion> { matchingPromotion, otherPromotion });

        productRepositoryMock
            .Setup(r => r.GetById(100))
            .Returns(product100);

        productRepositoryMock
            .Setup(r => r.GetById(200))
            .Returns(product200);

        var request = new PromotionSearchRequestDto
        {
            Date = date,
            ProductId = 100
        };

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var result = service.GetPromotions(request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Promo Producto 100", result[0].Name);
        Assert.IsFalse(result.Any(r => r.Name == "Promo Otro Producto"));

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenValidDateAndCommercialLine_WhenGettingPromotions_ThenReturnsOnlyPromotionsForThatCommercialLine()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var date = new DateOnly(2026, 1, 26);

        var matchingPromotion = Promotion.MakePromotion(
            "Promo Línea Matching",
            10,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );
        matchingPromotion.AssociateProduct(100);

        var otherPromotion = Promotion.MakePromotion(
            "Promo Línea Otra",
            15,
            new DateOnly(2026, 1, 25),
            new DateOnly(2026, 1, 30)
        );
        otherPromotion.AssociateProduct(200);

        var productMatchingLine = new ProductBuilder()
            .WithCode("COD100")
            .WithName("Producto Línea A")
            .WithDescription("Descripción válida con más de veinte caracteres")
            .WithCommercialLine("Línea A")
            .WithCategory("Categoría")
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto> { new ProductPhoto("photo-1.jpg") })
            .Build();

        var productOtherLine = new ProductBuilder()
            .WithCode("COD200")
            .WithName("Producto Línea B")
            .WithDescription("Descripción válida con más de veinte caracteres")
            .WithCommercialLine("Línea B")
            .WithCategory("Categoría")
            .WithPrice(100)
            .WithPhotos(new List<ProductPhoto> { new ProductPhoto("photo-2.jpg") })
            .Build();

        promotionRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(new List<Promotion> { matchingPromotion, otherPromotion });

        productRepositoryMock
            .Setup(r => r.GetById(100))
            .Returns(productMatchingLine);

        productRepositoryMock
            .Setup(r => r.GetById(200))
            .Returns(productOtherLine);

        var request = new PromotionSearchRequestDto
        {
            Date = date,
            CommercialLine = "Línea A"
        };

        var service = new PromotionService(
            promotionRepositoryMock.Object,
            productRepositoryMock.Object
        );

        var result = service.GetPromotions(request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Promo Línea Matching", result[0].Name);
        Assert.IsFalse(result.Any(r => r.Name == "Promo Línea Otra"));

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }
}
