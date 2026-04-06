using Contratos;
using DarkKitchen.Controllers;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Pruebas.WebApi.Users;

[TestClass]
public class UsersControllerTests
{
    [TestMethod]
    public void CreateUser_ValidRequest_ReturnsCreated()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>();

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

        var controller = new UsersController(signUpServiceMock.Object);

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
}
