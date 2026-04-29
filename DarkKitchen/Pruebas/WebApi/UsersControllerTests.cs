using System.Security.Claims;
using Contratos.Users;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using RouteData = Microsoft.AspNetCore.Routing.RouteData;

namespace Pruebas.WebApi;

[TestClass]
public class UsersControllerTests
{
    private Mock<IUserService> _userServiceMock = null!;
    private UsersController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        _controller = new UsersController(_userServiceMock.Object);
    }

    private void SetUserClaim(string? userIdClaimValue)
    {
        var claims = new List<Claim>();

        if(userIdClaimValue != null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userIdClaimValue));
        }

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "TestAuth"))
            }
        };
    }

    private static SingUpInternUserRequestDTO CreateValidInternalUserRequest()
    {
        return new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = UserRolesDto.Administrative.ToString()
        };
    }

    private static FindUserRequestDTO CreateFindUserRequest()
    {
        return new FindUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez"
        };
    }

    [TestMethod]
    public void DeleteInternalUser_CallsDeleteServiceOnce()
    {
        _userServiceMock
            .Setup(x => x.DeleteUser(2, 99));

        SetUserClaim("99");

        _controller.DeleteInternalUser(2);

        _userServiceMock.Verify(
            x => x.DeleteUser(2, 99),
            Times.Once);
    }

    [TestMethod]
    public void DeleteInternalUser_WhenClaimIsValid_ReturnsNoContent()
    {
        _userServiceMock
            .Setup(x => x.DeleteUser(2, 99));

        SetUserClaim("99");

        var result = _controller.DeleteInternalUser(2);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public void DeleteInternalUser_WhenClaimIsInvalid_ReturnsUnauthorized()
    {
        SetUserClaim("abc");

        var result = _controller.DeleteInternalUser(2);

        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        _userServiceMock.Verify(
            x => x.DeleteUser(It.IsAny<int>(), It.IsAny<int>()),
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
        var request = CreateValidInternalUserRequest();

        _userServiceMock
            .Setup(x => x.MakeUserIntern(99, request))
            .Returns((UserResponseDTO)null!);

        SetUserClaim("99");

        _controller.CreateInternalUser(request);

        _userServiceMock.Verify(
            x => x.MakeUserIntern(99, request),
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
        var request = CreateValidInternalUserRequest();

        var response = new UserResponseDTO
        {
            Id = 10,
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Role = UserRolesDto.Administrative.ToString()
        };

        _userServiceMock
            .Setup(x => x.MakeUserIntern(99, request))
            .Returns(response);

        SetUserClaim("99");

        var result = _controller.CreateInternalUser(request);

        var createdResult = result as CreatedResult;

        Assert.IsNotNull(createdResult);
        Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.AreEqual(response, createdResult.Value);
    }

    [TestMethod]
    public void CreateInternalUser_WhenClaimIsInvalid_ReturnsUnauthorized()
    {
        var request = CreateValidInternalUserRequest();

        SetUserClaim("abc");

        var result = _controller.CreateInternalUser(request) as UnauthorizedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

        _userServiceMock.Verify(
            x => x.MakeUserIntern(It.IsAny<int>(), It.IsAny<SingUpInternUserRequestDTO>()),
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
        var request = CreateFindUserRequest();
        var response = new List<UserResponseDTO>();

        _userServiceMock
            .Setup(x => x.FindUserOnSystem(99, request))
            .Returns(response);

        SetUserClaim("99");

        var result = _controller.SearchUsers(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        Assert.AreEqual(response, result.Value);

        _userServiceMock.Verify(
            x => x.FindUserOnSystem(99, request),
            Times.Once);
    }

    [TestMethod]
    public void SearchUsers_WhenClaimIsInvalid_ReturnsUnauthorized()
    {
        var request = CreateFindUserRequest();

        SetUserClaim("abc");

        var result = _controller.SearchUsers(request) as UnauthorizedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

        _userServiceMock.Verify(
            x => x.FindUserOnSystem(It.IsAny<int>(), It.IsAny<FindUserRequestDTO>()),
            Times.Never);
    }

    [TestMethod]
    public void SearchUsers_WhenClaimIsMissing_ReturnsUnauthorized()
    {
        var request = CreateFindUserRequest();

        SetUserClaim(null);

        var result = _controller.SearchUsers(request) as UnauthorizedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

        _userServiceMock.Verify(
            x => x.FindUserOnSystem(It.IsAny<int>(), It.IsAny<FindUserRequestDTO>()),
            Times.Never);
    }

    [TestMethod]
    public void SearchUsers_HasRoleAuthorizationAttribute()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.SearchUsers));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void UpdateInternalUserPassword_WhenRequestIsValid_ReturnsNoContent()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        userServiceMock
            .Setup(x => x.ModifyPasswordUser(3, 99, request));

        var controller = new UsersController(userServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, "99")
                ], "mock"))
            }
        };

        var result = controller.UpdateInternalUserPassword(3, request);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        userServiceMock.Verify(x => x.ModifyPasswordUser(3, 99, request), Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUserPassword_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var controller = new UsersController(userServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        var result = controller.UpdateInternalUserPassword(3, request);

        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        userServiceMock.Verify(
            x => x.ModifyPasswordUser(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<ModifyPasswordRequestDTO>()),
            Times.Never);
    }

    [TestMethod]
    public void UpdateInternalUserPassword_WhenUserIsAuthenticated_CallsService()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        userServiceMock
            .Setup(x => x.ModifyPasswordUser(3, 99, request));

        var controller = new UsersController(userServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, "99")
                ], "mock"))
            }
        };

        var result = controller.UpdateInternalUserPassword(3, request);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        userServiceMock.Verify(x => x.ModifyPasswordUser(3, 99, request), Times.Once);
    }

    [TestMethod]
    public void OnAuthorization_WhenUserIsClient_ReturnsForbiddenStatusCode()
    {
        var attribute = new RoleAuthorizationAttribute(UserRolesDto.Administrative);

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Client")
        ], "mock"));

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var filterContext = new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());

        attribute.OnAuthorization(filterContext);

        Assert.IsNotNull(filterContext.Result);
        Assert.IsInstanceOfType(filterContext.Result, typeof(ObjectResult));

        var result = (ObjectResult)filterContext.Result;
        Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);
    }
}
