using Contratos;
using Contratos.Users;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Pruebas.SignUp;

[TestClass]
public class ClientsControllerTests
{
    [TestMethod]
    public void CreateClient_ValidRequest_ReturnsCreated()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var expectedResponse = new SignUpClientResponseDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com"
        };

        signUpServiceMock
            .Setup(s => s.SignUpClient(request))
            .Returns(expectedResponse);

        var controller = new ClientsController(signUpServiceMock.Object);

        var result = controller.CreateClient(request);

        var createdResult = result as CreatedResult;

        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);

        var value = createdResult.Value as SignUpClientResponseDTO;
        Assert.IsNotNull(value);
        Assert.AreEqual("Miguel", value.Name);
        Assert.AreEqual("Lopez", value.LastName);
        Assert.AreEqual("miguel@gmail.com", value.Email);
    }

    [TestMethod]
    public void CreateClient_EmailAlreadyExists_ThrowsConflictException()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        signUpServiceMock
            .Setup(s => s.SignUpClient(request))
            .Throws(new ConflictException("Email is already registered"));

        var controller = new ClientsController(signUpServiceMock.Object);

        var ex = Assert.ThrowsException<ConflictException>(() =>
            controller.CreateClient(request));

        Assert.AreEqual("Email is already registered", ex.Message);

        signUpServiceMock.Verify(s => s.SignUpClient(request), Times.Once);
    }

    [TestMethod]
    public void CreateClient_InvalidPhone_ThrowsSignUpValidationException()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        signUpServiceMock
            .Setup(s => s.SignUpClient(request))
            .Throws(new ValidationException("Phone is not valid"));

        var controller = new ClientsController(signUpServiceMock.Object);

        var ex = Assert.ThrowsException<ValidationException>(() =>
            controller.CreateClient(request));

        Assert.AreEqual("Phone is not valid", ex.Message);

        signUpServiceMock.Verify(s => s.SignUpClient(request), Times.Once);
    }

    [TestMethod]
    public void CreateClient_ValidRequest_CallsServiceOnce()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var expectedResponse = new SignUpClientResponseDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com"
        };

        signUpServiceMock
            .Setup(s => s.SignUpClient(request))
            .Returns(expectedResponse);

        var controller = new ClientsController(signUpServiceMock.Object);

        controller.CreateClient(request);

        signUpServiceMock.Verify(s => s.SignUpClient(request), Times.Once);
    }

    [TestMethod]
    public void CreateClient_HasAllowAnonymousAttribute()
    {
        var method = typeof(ClientsController).GetMethod(nameof(ClientsController.CreateClient));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false)
            .Cast<AllowAnonymousAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void CreateClient_HasHttpPostAttribute()
    {
        var method = typeof(ClientsController).GetMethod(nameof(ClientsController.CreateClient));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(HttpPostAttribute), false)
            .Cast<HttpPostAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void CreateClient_ValidRequest_ReturnsCreatedWithCorrectLocation()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var expectedResponse = new SignUpClientResponseDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com"
        };

        signUpServiceMock
            .Setup(s => s.SignUpClient(request))
            .Returns(expectedResponse);

        var controller = new ClientsController(signUpServiceMock.Object);

        var result = controller.CreateClient(request) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual("/api/v1/clients", result.Location);
    }

    [TestMethod]
    public void CreateClient_DoesNotHaveRoleAuthorizationAttribute()
    {
        var method = typeof(ClientsController).GetMethod(nameof(ClientsController.CreateClient));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNull(attribute);
    }
}
