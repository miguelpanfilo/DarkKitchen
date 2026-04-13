using Contratos;
using DarkKitchen.Controllers;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Pruebas.SignUp;

[TestClass]
public class UsersControllerTests
{
    [TestMethod]
    public void CreateUser_ValidRequest_ReturnsCreated()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);

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

        var controller = new UsersController(signUpServiceMock.Object, modifyUserServiceMock.Object);

        var result = controller.CreateUser(request);

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
    public void CreateUser_EmailAlreadyExists_ThrowsSignUpException()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);

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
            .Throws(new SignUpException("Email is already registered"));

        var controller = new UsersController(signUpServiceMock.Object, modifyUserServiceMock.Object);

        var ex = Assert.ThrowsException<SignUpException>(() =>
            controller.CreateUser(request));

        Assert.AreEqual("Email is already registered", ex.Message);

        signUpServiceMock.Verify(s => s.SignUpClient(request), Times.Once);
    }

    [TestMethod]
    public void CreateUser_InvalidPhone_ThrowsSignUpValidationException()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);

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
            .Throws(new SignUpValidationException("Phone is not valid"));

        var controller = new UsersController(signUpServiceMock.Object, modifyUserServiceMock.Object);

        var ex = Assert.ThrowsException<SignUpValidationException>(() =>
            controller.CreateUser(request));

        Assert.AreEqual("Phone is not valid", ex.Message);

        signUpServiceMock.Verify(s => s.SignUpClient(request), Times.Once);
    }

    [TestMethod]
    public void CreateUser_ValidRequest_CallsServiceOnce()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);

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

        var controller = new UsersController(signUpServiceMock.Object, modifyUserServiceMock.Object);

        controller.CreateUser(request);

        signUpServiceMock.Verify(s => s.SignUpClient(request), Times.Once);
    }

    [TestMethod]
    public void CreateUser_HasAllowAnonymousAttribute()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.CreateUser));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false)
            .Cast<AllowAnonymousAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }
}
