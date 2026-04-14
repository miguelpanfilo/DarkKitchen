using System.Security.Claims;
using Contratos;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
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

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object);
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
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
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

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object);
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
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
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

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object);
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
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
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

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object);
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

    [TestMethod]
    public void CreateUser_HasHttpPostAttribute()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.CreateUser));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(HttpPostAttribute), false)
            .Cast<HttpPostAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void CreateUser_ValidRequest_ReturnsCreatedWithCorrectLocation()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
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

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object);
        var result = controller.CreateUser(request) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual("/api/v1/users", result.Location);
    }

    [TestMethod]
    public void CreateUser_DoesNotHaveRoleAuthorizationAttribute()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.CreateUser));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNull(attribute);
    }

    [TestMethod]
    public void DeleteInternalUser_CallsDeleteServiceOnce()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);

        deleteUserServiceMock
            .Setup(x => x.DeleteInterUser(2, 99));

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, "99")
                        }))
            }
        };

        controller.DeleteInternalUser(2);

        deleteUserServiceMock.Verify(x => x.DeleteInterUser(2, 99), Times.Once);
    }
}
