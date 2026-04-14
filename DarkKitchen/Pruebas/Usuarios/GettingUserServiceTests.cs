using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class GettingUserServiceTests
{
    [TestMethod]
    public void GetCurrentClient_CallsRepositoryFindByIdOnce()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);

        var phoneResult = Phone.TryMakePhone("099123456");
        Assert.IsTrue(phoneResult.Success);

        var userResult = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phoneResult.Value!,
            "PasswordSeguraee3!");

        Assert.IsTrue(userResult.Success);
        var user = userResult.Value!;
        user.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(user);

        var service = new GetClientService(repoMock.Object);

        service.GetCurrentClient(5);

        repoMock.Verify(r => r.FindById(5), Times.Once);
    }
}
