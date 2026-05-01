using System.Security.Claims;
using BusinessLogicsAbstractions.Users;
using Contract.Users;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

[TestClass]
public class ModifyUsersControllerTests
{
    private static HttpContext BuildHttpContextWithUserId(int userId)
    {
        var httpContext = new DefaultHttpContext();

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };

        httpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(claims, "TestAuth"));

        return httpContext;
    }

    [TestMethod]
    public void UpdateInternalUser_WhenRequestIsValid_ReturnsOk()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        var request = new ModifyUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Role = "Preparer"
        };

        var response = new UserResponseDTO
        {
            Id = 10,
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Role = "Preparer"
        };

        userServiceMock
            .Setup(x => x.ModifyUserIntern(10, 99, request))
            .Returns(response);

        var controller = new UsersController(userServiceMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = BuildHttpContextWithUserId(99)
        };

        var result = controller.UpdateInternalUser(10, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        Assert.AreEqual(response, result.Value);

        userServiceMock.Verify(
            x => x.ModifyUserIntern(10, 99, request),
            Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenRequestIsValid_ReturnsServiceResponse()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        var request = new ModifyUserRequestDTO { Name = "Juan" };
        var response = new UserResponseDTO { Id = 10, Name = "Juan" };

        userServiceMock
            .Setup(x => x.ModifyUserIntern(10, 99, request))
            .Returns(response);

        var controller = new UsersController(userServiceMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = BuildHttpContextWithUserId(99)
        };

        var result = controller.UpdateInternalUser(10, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreSame(response, result.Value);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenCalled_UsesRouteId()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        var request = new ModifyUserRequestDTO();
        var response = new UserResponseDTO();

        userServiceMock
            .Setup(x => x.ModifyUserIntern(25, 99, request))
            .Returns(response);

        var controller = new UsersController(userServiceMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = BuildHttpContextWithUserId(99)
        };

        controller.UpdateInternalUser(25, request);

        userServiceMock.Verify(
            x => x.ModifyUserIntern(25, 99, request),
            Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenCalled_UsesLoggedUserIdFromClaims()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        var controller = new UsersController(userServiceMock.Object);

        var request = new ModifyUserRequestDTO();
        var response = new UserResponseDTO();

        userServiceMock
            .Setup(x => x.ModifyUserIntern(10, 77, request))
            .Returns(response);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = BuildHttpContextWithUserId(77)
        };

        var result = controller.UpdateInternalUser(10, request);

        userServiceMock.Verify(
            x => x.ModifyUserIntern(10, 77, request),
            Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_HasHttpPatchAttributeWithIdTemplate()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.UpdateInternalUser));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(HttpPatchAttribute), false)
            .Cast<HttpPatchAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
        Assert.AreEqual("{id}", attribute.Template);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenNameIdentifierClaimIsMissing_ReturnsUnauthorized()
    {
        var userService = new Mock<IUserService>(MockBehavior.Strict);
        var controller = new UsersController(
            userService.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([], "TestAuth"))
            }
        };

        var request = new ModifyUserRequestDTO();

        var result = controller.UpdateInternalUser(10, request) as UnauthorizedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public void UpdateInternalUser_HasRoleAuthorizationAttribute()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.UpdateInternalUser));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenNameIdentifierIsNotNumeric_ReturnsUnauthorized()
    {
        var userService = new Mock<IUserService>(MockBehavior.Strict);
        var controller = new UsersController(
            userService.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, "abc")],
                "TestAuth"));

       controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var request = new ModifyUserRequestDTO();

        var result = controller.UpdateInternalUser(10, request) as UnauthorizedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenClaimIsMissing_DoesNotCallService()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        var controller = new UsersController(userServiceMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([], "TestAuth"))
            }
        };

        var request = new ModifyUserRequestDTO();

        var result = controller.UpdateInternalUser(10, request) as UnauthorizedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

        userServiceMock.Verify(
            x => x.ModifyUserIntern(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<ModifyUserRequestDTO>()),
            Times.Never);
    }
}
