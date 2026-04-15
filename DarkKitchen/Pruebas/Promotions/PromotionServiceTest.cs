using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.Promotions;

[TestClass]
public class PromotionServiceTest
{
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

        repositoryMock
            .Setup(r => r.Create(It.IsAny<Promotion>()))
            .Returns((Promotion promotion) => promotion);

        var service = new PromotionService(repositoryMock.Object);

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
        var service = new PromotionService(repositoryMock.Object);

        var ex = Assert.ThrowsException<PromotionException>(() => service.Create(promotionDto));

        Assert.AreEqual("El nombre de la promoción no puede ser vacío", ex.Message);
    }
}
