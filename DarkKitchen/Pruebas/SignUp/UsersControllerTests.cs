using System.Security.Claims;
using Contratos;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Pruebas.SignUp;

[TestClass]
public class UsersControllerTests
{
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

    [TestMethod]
    public void DeleteInternalUser_WhenClaimIsValid_ReturnsNoContent()
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

        var result = controller.DeleteInternalUser(2);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public void DeleteInternalUser_WhenClaimIsInvalid_ReturnsUnauthorized()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);

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
                            new Claim(ClaimTypes.NameIdentifier, "abc")
                        }))
            }
        };

        var result = controller.DeleteInternalUser(2);

        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        deleteUserServiceMock.Verify(
            x => x.DeleteInterUser(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [TestMethod]
    public void DeleteInternalUser_HasHttpDeleteAttribute()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.DeleteInternalUser));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(HttpDeleteAttribute), false)
            .Cast<HttpDeleteAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void DeleteInternalUser_HasRoleAuthorizationAttribute()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.DeleteInternalUser));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void DeleteInternalUser_HasHttpDeleteAttributeWithIdTemplate()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.DeleteInternalUser));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(HttpDeleteAttribute), false)
            .Cast<HttpDeleteAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
        Assert.AreEqual("{id}", attribute.Template);
    }

    [TestMethod]
    public void CreateInternalUser_CallsSignUpServiceOnce()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = UserRolesDto.Administrative.ToString()
        };

        signUpServiceMock
            .Setup(x => x.MakeInterUser(99, request))
            .Returns((UsuarioResponseDTO)null!);

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "99")
        ]));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };

        controller.CreateInternalUser(request);

        signUpServiceMock.Verify(
            x => x.MakeInterUser(99, request),
            Times.Once);
    }

    [TestMethod]
    public void CreateInternalUser_HasRoleAuthorizationAttribute()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.CreateInternalUser));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void CreateInternalUser_ValidRequest_ReturnsCreatedWithResponse()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = UserRolesDto.Administrative.ToString()
        };

        var response = new UsuarioResponseDTO
        {
            Id = 10,
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Role = UserRolesDto.Administrative.ToString()
        };

        signUpServiceMock
            .Setup(x => x.MakeInterUser(99, request))
            .Returns(response);

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "99")
        ]));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };

        var result = controller.CreateInternalUser(request);

        var createdResult = result as CreatedResult;

        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);
        Assert.AreEqual(response, createdResult.Value);
    }

    [TestMethod]
    public void CreateInternalUser_WhenClaimIsInvalid_ReturnsUnauthorized()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = UserRolesDto.Administrative.ToString()
        };

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
                            new Claim(ClaimTypes.NameIdentifier, "abc")
                        }))
            }
        };

        var result = controller.CreateInternalUser(request);

        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        signUpServiceMock.Verify(
            x => x.MakeInterUser(It.IsAny<int>(), It.IsAny<SingUpInternUserRequestDTO>()),
            Times.Never);
    }

    [TestMethod]
    public void CreateInternalUser_HasHttpPostAttribute()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.CreateInternalUser));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(HttpPostAttribute), false)
            .Cast<HttpPostAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void SearchUsers_CallsFilterServiceOnce()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
        var filterUserServiceMock = new Mock<IFilterUserService>(MockBehavior.Strict);

        var request = new FindUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez"
        };

        filterUserServiceMock
            .Setup(x => x.BuscarUsuarios(99, request))
            .Returns(new List<UsuarioResponseDTO>());

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object,
            filterUserServiceMock.Object);

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

        controller.SearchUsers(request);

        filterUserServiceMock.Verify(
            x => x.BuscarUsuarios(99, request),
            Times.Once);
    }
}
