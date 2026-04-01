using AlmacenamientoAbstracciones;
using Dominio;
using Moq;

namespace Pruebas.InicioSesion;

[TestClass]
public class AuthServiceTests
{
    [TestMethod]
    public void Login_CredencialesValidas_RetornaSesion()
    {
        var repoUsuarioMock = new Mock<IRepositorioUsuario>();
        var repoSesionMock = new Mock<IRepositorioSesion>();

        var usuario = Usuario.CrearCliente(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2Ok6Abc"
        );
        usuario.Id = 1;

        repoUsuarioMock
            .Setup(r => r.ObtenerPorEmail("miguel@gmail.com"))
            .Returns(usuario);

        repoSesionMock
            .Setup(r => r.Add(It.IsAny<Sesion>()))
            .Returns((Sesion s) => s);

        var service = new AuthService(repoUsuarioMock.Object, repoSesionMock.Object);

        var request = new LoginRequest
        {
            Email = "miguel@gmail.com",
            Clave = "Password@2Ok6Abc"
        };

        var response = service.Login(request);

        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.UsuarioId);
        Assert.AreEqual("Cliente", response.Rol);
        Assert.AreNotEqual(Guid.Empty, response.SessionId);
    }
}
