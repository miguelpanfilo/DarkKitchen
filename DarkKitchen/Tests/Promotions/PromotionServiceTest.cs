using BusinessLogics;
using BusinessLogicsAbstractions;
using Contract.Promotions;
using DataAccessAbstractions;
using Domain;
using Domain.Builders;
using Moq;

namespace Tests.Promotions;

[TestClass]
public class PromotionServiceTest
{
    private DateOnly Today() =>
        DateOnly.FromDateTime(DateTime.Now);

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

    private static PromotionService CreateService(
        Mock<IPromotionRepository> promotionRepo,
        Mock<IProductRepository> productRepo,
        Mock<IAuditLogService> auditMock)
    {
        var currentUserMock = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUserMock.Setup(s => s.GetEmail()).Returns("admin@dark.com");
        return new PromotionService(promotionRepo.Object, productRepo.Object,
            auditMock.Object, currentUserMock.Object);
    }

    [TestMethod]
    public void GivenValidPromotionDto_WhenCreating_ThenRepositoryIsCalledAndPromotionIsReturned()
    {
        var (start, end) = FutureDates();
        var promotionDto = new PromotionRequestDto
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            StartDate = start,
            EndDate = end
        };

        var repositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        repositoryMock
            .Setup(r => r.Create(It.IsAny<Promotion>()))
            .Returns((Promotion promotion) => promotion);

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(repositoryMock, productRepositoryMock, auditMock);
        var result = service.Create(promotionDto);;

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
        var (start, end) = FutureDates();
        var promotionDto = new PromotionRequestDto
        {
            Name = invalidName,
            DiscountPercentage = 10,
            StartDate = start,
            EndDate = end
        };

        var repositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(repositoryMock,
            productRepositoryMock, auditMock);
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
        var (start, end) = FutureDates();
        var promotionDto = new PromotionRequestDto
        {
            Name = "Black Friday",
            DiscountPercentage = (decimal)invalidDiscount,
            StartDate = start,
            EndDate = end
        };

        var repositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(repositoryMock,
            productRepositoryMock, auditMock);
        var ex = Assert.ThrowsException<ValidationException>(() => service.Create(promotionDto));

        Assert.AreEqual("El porcentaje de descuento debe estar entre 1 y 100", ex.Message);
    }

    [TestMethod]
    public void GivenPromotionDtoWithInvalidDateRange_WhenCreating_ThenThrowsPromotionException()
    {
        var today = Today();
        var promotionDto = new PromotionRequestDto
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            StartDate = today.AddDays(10),
            EndDate = today.AddDays(5)
        };

        var repositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(repositoryMock,
            productRepositoryMock, auditMock);
        var ex = Assert.ThrowsException<ValidationException>(() => service.Create(promotionDto));

        Assert.AreEqual("La fecha de inicio no puede ser mayor a la fecha de fin", ex.Message);
    }

    [TestMethod]
    public void GivenExistingPromotionAndProduct_WhenAssociatingProduct_ThenProductIsAddedToPromotion()
    {
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);

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

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

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

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(promotionRepositoryMock,
            productRepositoryMock, auditMock);

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
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        productRepositoryMock
            .Setup(r => r.GetById(100))
            .Returns((Product)null!);

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

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
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);
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

        var auditMock = new Mock<IAuditLogService>();
        var servicePromotion = CreateService(promotionRepositoryMock, productRepositoryMock, auditMock);

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
        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

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
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        productRepositoryMock
            .Setup(r => r.GetById(100))
            .Returns((Product)null!);
        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

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

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);
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

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

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

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

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

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

        var ex = Assert.ThrowsException<ValidationException>(() => service.Update(1, patchDto));

        Assert.AreEqual("El porcentaje de descuento debe estar entre 1 y 100", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithValidStartDate_ThenPromotionStartDateIsUpdated()
    {
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);

        var newStartDate = start.AddDays(1);

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

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(newStartDate, result.StartDate);
        Assert.AreEqual(end, result.EndDate);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithValidEndDate_ThenPromotionEndDateIsUpdated()
    {
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);

        var newEndDate = end.AddDays(1);

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

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(start, result.StartDate);
        Assert.AreEqual(newEndDate, result.EndDate);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithValidStartDateAndEndDate_ThenPromotionDatesAreUpdated()
    {
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);

        var newStartDate = start.AddDays(1);
        var newEndDate = end.AddDays(1);

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

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

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
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);

        var patchDto = new PromotionPatchDto
        {
            StartDate = end.AddDays(1)
        };

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

        var ex = Assert.ThrowsException<ValidationException>(() => service.Update(1, patchDto));

        Assert.AreEqual("La fecha de inicio no puede ser mayor a la fecha de fin", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithInvalidEndDate_ThenThrowsPromotionException()
    {
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);

        var patchDto = new PromotionPatchDto
        {
            EndDate = start.AddDays(-1)
        };

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

        var ex = Assert.ThrowsException<ValidationException>(() => service.Update(1, patchDto));

        Assert.AreEqual("La fecha de inicio no puede ser mayor a la fecha de fin", ex.Message);

        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithInvalidDateRange_ThenThrowsPromotionException()
    {
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);

        var patchDto = new PromotionPatchDto
        {
            StartDate = end.AddDays(5),
            EndDate = end.AddDays(1)
        };

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

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

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

        var ex = Assert.ThrowsException<NotFoundException>(() => service.Update(1, patchDto));

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

        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        promotionRepositoryMock
            .Setup(r => r.Update(It.Is<Promotion>(p => p.IsDeleted)))
            .Returns(promotion);

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

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

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

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

        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);

        promotion.Delete();

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);
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

        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);
        promotion.Delete();

        var patchDto = new PromotionPatchDto
        {
            Name = "Cyber Monday"
        };

        promotionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);
        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);
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

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

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

        var today = Today();
        var filterDate = today.AddDays(5);

        var activePromotion = Promotion.MakePromotion(
            "Promo Activa",
            10,
            today.AddDays(3),
            today.AddDays(10)
        );

        var inactivePromotion = Promotion.MakePromotion(
            "Promo No Iniciada",
            10,
            today.AddDays(8),
            today.AddDays(15)
        );

        promotionRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(new List<Promotion> { activePromotion, inactivePromotion });

        var request = new PromotionSearchRequestDto
        {
            Date = filterDate
        };

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

        var result = service.GetPromotions(request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Promo Activa", result[0].Name);

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetHighestCurrentDiscountForProduct_WhenProductHasMultipleCurrentPromotions_ShouldReturnHighestDiscount()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var today = DateOnly.FromDateTime(DateTime.Now);

        var promo10 = Promotion.MakePromotion(
            "Promo 10",
            10,
            today.AddDays(-1),
            today.AddDays(1));
        promo10.AssociateProduct(100);

        var promo25 = Promotion.MakePromotion(
            "Promo 25",
            25,
            today.AddDays(-1),
            today.AddDays(1));
        promo25.AssociateProduct(100);

        var promoOtherProduct = Promotion.MakePromotion(
            "Promo Otro Producto",
            50,
            today.AddDays(-1),
            today.AddDays(1));
        promoOtherProduct.AssociateProduct(200);

        promotionRepositoryMock
            .Setup(r => r.GetAll())
            .Returns([promo10, promo25, promoOtherProduct]);

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

        var result = service.GetHighestCurrentDiscountForProduct(100);

        Assert.AreEqual(0.25m, result);

        promotionRepositoryMock.Verify(r => r.GetAll(), Times.Once);
        productRepositoryMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void GivenValidDateAndProductId_WhenGettingPromotions_ThenReturnsOnlyPromotionsForThatProduct()
    {
        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

        var today = Today();
        var filterDate = today.AddDays(5);

        var matchingPromotion = Promotion.MakePromotion(
            "Promo Producto 100",
            10,
            today.AddDays(3),
            today.AddDays(10)
        );
        matchingPromotion.AssociateProduct(100);

        var otherPromotion = Promotion.MakePromotion(
            "Promo Otro Producto",
            15,
            today.AddDays(3),
            today.AddDays(10)
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
            Date = filterDate,
            ProductId = 100
        };

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

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

        var today = Today();
        var filterDate = today.AddDays(5);

        var matchingPromotion = Promotion.MakePromotion(
            "Promo Línea Matching",
            10,
            today.AddDays(3),
            today.AddDays(10)
        );
        matchingPromotion.AssociateProduct(100);

        var otherPromotion = Promotion.MakePromotion(
            "Promo Línea Otra",
            15,
            today.AddDays(3),
            today.AddDays(10)
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
            Date = filterDate,
            CommercialLine = "Línea A"
        };

        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(
            promotionRepositoryMock,
            productRepositoryMock, auditMock);

        var result = service.GetPromotions(request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Promo Línea Matching", result[0].Name);
        Assert.IsFalse(result.Any(r => r.Name == "Promo Línea Otra"));

        promotionRepositoryMock.VerifyAll();
        productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithNoChanges_ThenRepositoryIsNotCalledAndNoAuditIsRegistered()
    {
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);
        var patchDto = new PromotionPatchDto();

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
        promotionRepositoryMock.Setup(r => r.GetById(1)).Returns(promotion);

        var auditMock = new Mock<IAuditLogService>(MockBehavior.Strict);
        var service = CreateService(promotionRepositoryMock, productRepositoryMock, auditMock);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);
        auditMock.Verify(r => r.Register(
            It.IsAny<string>(), It.IsAny<int>(),
            It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void GivenExistingPromotion_WhenUpdatingWithSameValues_ThenRepositoryIsNotCalledAndNoAuditIsRegistered()
    {
        var (start, end) = FutureDates();
        var promotion = Promotion.MakePromotion("Black Friday", 10, start, end);

        var patchDto = new PromotionPatchDto
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            StartDate = start,
            EndDate = end
        };

        var promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
        promotionRepositoryMock.Setup(r => r.GetById(1)).Returns(promotion);

        var auditMock = new Mock<IAuditLogService>(MockBehavior.Strict);
        var service = CreateService(promotionRepositoryMock, productRepositoryMock, auditMock);

        var result = service.Update(1, patchDto);

        Assert.IsNotNull(result);
        promotionRepositoryMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Never);
        auditMock.Verify(r => r.Register(
            It.IsAny<string>(), It.IsAny<int>(),
            It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void GivenPromotionDtoWithEndDateInThePast_WhenCreating_ThenThrowsValidationException()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var promotionDto = new PromotionRequestDto
        {
            Name = "Promo Pasada",
            DiscountPercentage = 10,
            StartDate = today.AddDays(-10),
            EndDate = today.AddDays(-1)
        };

        var repositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var auditMock = new Mock<IAuditLogService>();
        var service = CreateService(repositoryMock, productRepositoryMock, auditMock);

        var ex = Assert.ThrowsException<ValidationException>(() => service.Create(promotionDto));
        Assert.AreEqual("La fecha de fin de la promoción no puede ser anterior a hoy", ex.Message);
    }
}
