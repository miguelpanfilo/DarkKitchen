using System.Security.Claims;
using BusinessLogicsAbstractions;
using Contract;
using DarkKitchen.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

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

        if (userIdClaimValue != null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userIdClaimValue));
        }

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            }
        };
    }

    private static SignUpInternUserRequestDTO CreateValidInternalUserRequest() =>
        new SignUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = UserRolesDto.Administrative.ToString()
        };

    private static FindUserRequestDTO CreateFindUserRequest() =>
        new FindUserRequestDTO { Name = "Juan", LastName = "Perez" };

    [TestMethod]
    public void DeleteInternalUser_CallsDeleteServiceOnce()
    {
        _userServiceMock.Setup(x => x.DeleteUser(2, 99));
        SetUserClaim("99");

        _controller.DeleteInternalUser(2);

        _userServiceMock.Verify(x => x.DeleteUser(2, 99), Times.Once);
    }

    [TestMethod]
    public void DeleteInternalUser_WhenClaimIsValid_ReturnsNoContent()
    {
        _userServiceMock.Setup(x => x.DeleteUser(2, 99));
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
        _userServiceMock.Verify(x => x.DeleteUser(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void CreateInternalUser_CallsSignUpServiceOnce()
    {
        var request = CreateValidInternalUserRequest();

        _userServiceMock.Setup(x => x.MakeUserIntern(99, request)).Returns((UserResponseDTO)null!);

        SetUserClaim("99");

        _controller.CreateInternalUser(request);

        _userServiceMock.Verify(x => x.MakeUserIntern(99, request), Times.Once);
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

        _userServiceMock.Setup(x => x.MakeUserIntern(99, request)).Returns(response);

        SetUserClaim("99");

        var result = _controller.CreateInternalUser(request) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        Assert.AreEqual(response, result.Value);
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
            x => x.MakeUserIntern(It.IsAny<int>(), It.IsAny<SignUpInternUserRequestDTO>()),
            Times.Never);
    }

    [TestMethod]
    public void SearchUsers_CallsFilterServiceOnce()
    {
        var request = CreateFindUserRequest();
        var response = new List<UserResponseDTO>();

        _userServiceMock.Setup(x => x.FindUserOnSystem(99, request)).Returns(response);

        SetUserClaim("99");

        var result = _controller.SearchUsers(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        Assert.AreEqual(response, result.Value);

        _userServiceMock.Verify(x => x.FindUserOnSystem(99, request), Times.Once);
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
    public void UpdateInternalUserPassword_WhenRequestIsValid_ReturnsNoContent()
    {
        var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        userServiceMock.Setup(x => x.ModifyPasswordUser(3, 99, request));

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
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
        };

        var result = controller.UpdateInternalUserPassword(3, request);

        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        userServiceMock.Verify(
            x => x.ModifyPasswordUser(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ModifyPasswordRequestDTO>()),
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

        userServiceMock.Setup(x => x.ModifyPasswordUser(3, 99, request));

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
    public void GetUserById_WhenClaimIsValid_ReturnsOkWithUser()
    {
        var response = new UserResponseDTO
        {
            Id = 5,
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Role = UserRolesDto.Administrative.ToString()
        };

        _userServiceMock.Setup(x => x.GetByIdIncludingInactive(5)).Returns(response);
        SetUserClaim("99");

        var result = _controller.GetUserById(5) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        Assert.AreEqual(response, result.Value);
    }
}
