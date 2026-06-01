using BusinessLogics;
using BusinessLogicsAbstractions;
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

    [TestMethod]
    public void GivenExistingShippingType_WhenUpdating_ThenRepositoryIsCalledAndResponseIsReturned()
    {
        var shippingType = ShippingType.MakeShippingType("Envío express", 150m);
        var patchDto = new ShippingTypePatchDto { Name = "Envío premium", Cost = 200m };

        var repositoryMock = CreateRepositoryMock();
        repositoryMock.Setup(r => r.GetById(1)).Returns(shippingType);
        repositoryMock.Setup(r => r.Update(shippingType)).Returns(shippingType);

        var service = new ShippingTypeService(repositoryMock.Object);

        var result = service.UpdateShippingType(1, patchDto);

        Assert.IsNotNull(result);
        Assert.AreEqual("Envío premium", result.Name);
        Assert.AreEqual(200m, result.Cost);
        repositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenNonExistingShippingType_WhenUpdating_ThenThrowsNotFoundException()
    {
        var patchDto = new ShippingTypePatchDto { Name = "Envío premium" };

        var repositoryMock = CreateRepositoryMock();
        repositoryMock.Setup(r => r.GetById(1)).Returns((ShippingType)null!);

        var service = new ShippingTypeService(repositoryMock.Object);

        var ex = Assert.ThrowsException<NotFoundException>(() => service.UpdateShippingType(1, patchDto));

        Assert.AreEqual("El tipo de envío no existe", ex.Message);
        repositoryMock.Verify(r => r.Update(It.IsAny<ShippingType>()), Times.Never);
        repositoryMock.VerifyAll();
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void GivenEmptyName_WhenUpdatingShippingType_ThenThrowsValidationException(string invalidName)
    {
        var shippingType = ShippingType.MakeShippingType("Envío express", 150m);
        var patchDto = new ShippingTypePatchDto { Name = invalidName };

        var repositoryMock = CreateRepositoryMock();
        repositoryMock.Setup(r => r.GetById(1)).Returns(shippingType);

        var service = new ShippingTypeService(repositoryMock.Object);

        var ex = Assert.ThrowsException<ValidationException>(() => service.UpdateShippingType(1, patchDto));

        Assert.AreEqual("El nombre del tipo de envío no puede estar vacío.", ex.Message);
        repositoryMock.Verify(r => r.Update(It.IsAny<ShippingType>()), Times.Never);
        repositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenNegativeCost_WhenUpdatingShippingType_ThenThrowsValidationException()
    {
        var shippingType = ShippingType.MakeShippingType("Envío express", 150m);
        var patchDto = new ShippingTypePatchDto { Cost = -1m };

        var repositoryMock = CreateRepositoryMock();
        repositoryMock.Setup(r => r.GetById(1)).Returns(shippingType);

        var service = new ShippingTypeService(repositoryMock.Object);

        var ex = Assert.ThrowsException<ValidationException>(() => service.UpdateShippingType(1, patchDto));

        Assert.AreEqual("El costo del tipo de envío no puede ser negativo.", ex.Message);
        repositoryMock.Verify(r => r.Update(It.IsAny<ShippingType>()), Times.Never);
        repositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenExistingShippingTypes_WhenGettingAll_ThenReturnsAllMapped()
    {
        var shippingTypes = new List<ShippingType>
        {
            ShippingType.MakeShippingType("Envío express", 150m),
            ShippingType.MakeShippingType("Envío estándar", 80m)
        };

        var repositoryMock = CreateRepositoryMock();
        repositoryMock.Setup(r => r.GetAll()).Returns(shippingTypes);

        var service = new ShippingTypeService(repositoryMock.Object);

        var result = service.GetAllShippingTypes();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Envío express", result[0].Name);
        Assert.AreEqual(150m, result[0].Cost);
        Assert.AreEqual("Envío estándar", result[1].Name);
        Assert.AreEqual(80m, result[1].Cost);
        repositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GivenNoShippingTypes_WhenGettingAll_ThenReturnsEmptyList()
    {
        var repositoryMock = CreateRepositoryMock();
        repositoryMock.Setup(r => r.GetAll()).Returns(new List<ShippingType>());

        var service = new ShippingTypeService(repositoryMock.Object);

        var result = service.GetAllShippingTypes();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
        repositoryMock.VerifyAll();
    }
}
