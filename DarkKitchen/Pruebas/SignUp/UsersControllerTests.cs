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
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        userServiceMock
            .Setup(x => x.EliminarUsuarioInterno(2, 99));

        var controller = new UsersController(userServiceMock.Object);

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

        userServiceMock.Verify(
            x => x.EliminarUsuarioInterno(2, 99),
            Times.Once);
    }

    [TestMethod]
    public void DeleteInternalUser_WhenClaimIsValid_ReturnsNoContent()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        userServiceMock
            .Setup(x => x.EliminarUsuarioInterno(2, 99));

        var controller = new UsersController(userServiceMock.Object);

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
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        var controller = new UsersController(userServiceMock.Object);

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
        userServiceMock.Verify(
            x => x.EliminarUsuarioInterno(It.IsAny<int>(), It.IsAny<int>()),
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
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = UserRolesDto.Administrative.ToString()
        };

        userServiceMock
            .Setup(x => x.CrearUsuarioInterno(99, request))
            .Returns((UsuarioResponseDTO)null!);

        var controller = new UsersController(userServiceMock.Object);

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

        userServiceMock.Verify(
            x => x.CrearUsuarioInterno(99, request),
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
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

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

        userServiceMock
            .Setup(x => x.CrearUsuarioInterno(99, request))
            .Returns(response);

        var controller = new UsersController(userServiceMock.Object);

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
        Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.AreEqual(response, createdResult.Value);
    }

    [TestMethod]
    public void CreateInternalUser_WhenClaimIsInvalid_ReturnsUnauthorized()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = UserRolesDto.Administrative.ToString()
        };

        var controller = new UsersController(userServiceMock.Object);

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

        var result = controller.CreateInternalUser(request) as UnauthorizedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

        userServiceMock.Verify(
            x => x.CrearUsuarioInterno(It.IsAny<int>(), It.IsAny<SingUpInternUserRequestDTO>()),
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
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        var request = new FindUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez"
        };

        var response = new List<UsuarioResponseDTO>();

        userServiceMock
            .Setup(x => x.BuscarUsuarios(99, request))
            .Returns(response);

        var controller = new UsersController(userServiceMock.Object);

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

        var result = controller.SearchUsers(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        Assert.AreEqual(response, result.Value);

        userServiceMock.Verify(
            x => x.BuscarUsuarios(99, request),
            Times.Once);
    }

    [TestMethod]
    public void SearchUsers_WhenClaimIsInvalid_ReturnsUnauthorized()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        var controller = new UsersController(userServiceMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez"
        };

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

        var result = controller.SearchUsers(request) as UnauthorizedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

        userServiceMock.Verify(
            x => x.BuscarUsuarios(It.IsAny<int>(), It.IsAny<FindUserRequestDTO>()),
            Times.Never);
    }

    [TestMethod]
    public void SearchUsers_WhenClaimIsMissing_ReturnsUnauthorized()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        var controller = new UsersController(userServiceMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez"
        };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([], "TestAuth"))
            }
        };

        var result = controller.SearchUsers(request) as UnauthorizedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

        userServiceMock.Verify(
            x => x.BuscarUsuarios(It.IsAny<int>(), It.IsAny<FindUserRequestDTO>()),
            Times.Never);
    }

    [TestMethod]
    public void SearchUsers_HasRoleAuthorizationForAdministrative()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.SearchUsers));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .FirstOrDefault();

        Assert.IsNotNull(attribute);
        Assert.AreEqual(UserRolesDto.Administrative, attribute.Role);
    }
}
