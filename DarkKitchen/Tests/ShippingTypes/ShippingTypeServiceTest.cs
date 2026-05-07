using BusinessLogics.ShippingTypes;
using BusinessLogicsAbstractions.Exceptions;
using Contract.ShippingTypes;
using DataAccessAbstractions;
using Domain;
using Moq;

namespace Tests.ShippingTypes;

[TestClass]
public class ShippingTypeServiceTest
{
    private static Mock<IShippingTypeRepository> CreateRepositoryMock()
    {
        return new Mock<IShippingTypeRepository>(MockBehavior.Strict);
    }

    private static ShippingTypeService CreateService()
    {
        return new ShippingTypeService(CreateRepositoryMock().Object);
    }

    [TestMethod]
    public void GivenValidRequest_WhenCreatingShippingType_ThenRepositoryIsCalledAndResponseIsReturned()
    {
        var request = new ShippingTypeRequestDto { Name = "Envío express", Cost = 150m };

        var repositoryMock = CreateRepositoryMock();
        repositoryMock
            .Setup(r => r.Create(It.IsAny<ShippingType>()))
            .Returns((ShippingType s) => s);

        var service = new ShippingTypeService(repositoryMock.Object);

        var result = service.CreateShippingType(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(request.Name, result.Name);
        Assert.AreEqual(request.Cost, result.Cost);
        repositoryMock.Verify(r => r.Create(It.IsAny<ShippingType>()), Times.Once);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void GivenEmptyName_WhenCreatingShippingType_ThenThrowsValidationException(string invalidName)
    {
        var request = new ShippingTypeRequestDto { Name = invalidName, Cost = 150m };

        var service = CreateService();

        var ex = Assert.ThrowsException<ValidationException>(() => service.CreateShippingType(request));

        Assert.AreEqual("El nombre del tipo de envío no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenNegativeCost_WhenCreatingShippingType_ThenThrowsValidationException()
    {
        var request = new ShippingTypeRequestDto { Name = "Envío express", Cost = -1m };

        var service = CreateService();

        var ex = Assert.ThrowsException<ValidationException>(() => service.CreateShippingType(request));

        Assert.AreEqual("El costo del tipo de envío no puede ser negativo.", ex.Message);
    }
}
