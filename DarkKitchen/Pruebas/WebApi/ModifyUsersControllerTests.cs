using Contratos;
using DarkKitchen.Controllers;
using LogicaDeNegocioAbstracciones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Pruebas.WebApi;

[TestClass]
public class ModifyUsersControllerTests
{
    private static HttpContext BuildHttpContextWithUserId(int userId)
    {
        var httpContext = new DefaultHttpContext();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        httpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(claims, "TestAuth"));

        return httpContext;
    }

    [TestMethod]
    public void UpdateInternalUser_WhenRequestIsValid_ReturnsOk()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);

        var request = new ModificarUsuarioRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Role = "Preparer"
        };

        var response = new UsuarioResponseDTO
        {
            Id = 10,
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Role = "Preparer"
        };

        modifyUserServiceMock
            .Setup(x => x.UpdateInternalUser(10, 99, request))
            .Returns(response);

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = BuildHttpContextWithUserId(99)
        };

        var result = controller.UpdateInternalUser(10, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        Assert.AreEqual(response, result.Value);

        modifyUserServiceMock.Verify(x => x.UpdateInternalUser(10, 99, request), Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenRequestIsValid_ReturnsServiceResponse()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);

        var request = new ModificarUsuarioRequestDTO
        {
            Name = "Juan"
        };

        var response = new UsuarioResponseDTO
        {
            Id = 10,
            Name = "Juan"
        };

        modifyUserServiceMock
            .Setup(x => x.UpdateInternalUser(10, 99, request))
            .Returns(response);

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object);

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
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);

        var request = new ModificarUsuarioRequestDTO();
        var response = new UsuarioResponseDTO();

        modifyUserServiceMock
            .Setup(x => x.UpdateInternalUser(25, 99, request))
            .Returns(response);

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = BuildHttpContextWithUserId(99)
        };

        controller.UpdateInternalUser(25, request);

        modifyUserServiceMock.Verify(x => x.UpdateInternalUser(25, 99, request), Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenCalled_UsesLoggedUserIdFromClaims()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);

        var request = new ModificarUsuarioRequestDTO();
        var response = new UsuarioResponseDTO();

        modifyUserServiceMock
            .Setup(x => x.UpdateInternalUser(10, 77, request))
            .Returns(response);

        var controller = new UsersController(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = BuildHttpContextWithUserId(77)
        };

        controller.UpdateInternalUser(10, request);

        modifyUserServiceMock.Verify(x => x.UpdateInternalUser(10, 77, request), Times.Once);
    }
}
