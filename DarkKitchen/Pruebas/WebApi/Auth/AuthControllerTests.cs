using Contratos;
using DarkKitchen.Controladores;
using LogicaDeNegocioAbstracciones;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Pruebas.WebApi.Auth;

[TestClass]
public class AuthControllerTests
{
    [TestMethod]
    public void Login_ValidRequest_ReturnsOk()
    {
        var loginServiceMock = new Mock<ILoginService>();

        var request = new LoginRequestDTO
        {
            Email = "admin@gmail.com",
            Password = "Password@123Abc"
        };

        var expectedResponse = new LoginResponseDTO
        {
            SessionId = Guid.NewGuid(),
            UserId = 1,
            Role = "Administrative",
            ExpireDate = new DateTime(2026, 4, 6, 18, 0, 0)
        };

        loginServiceMock
            .Setup(s => s.Login(request))
            .Returns(expectedResponse);

        var controller = new AuthController(loginServiceMock.Object);

        var result = controller.Login(request);

        var okResult = result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var value = okResult.Value as LoginResponseDTO;
        Assert.IsNotNull(value);
        Assert.AreEqual(1, value.UserId);
        Assert.AreEqual("Administrative", value.Role);
    }
}
