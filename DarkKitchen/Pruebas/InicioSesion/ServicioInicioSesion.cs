using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.InicioSesion;

[TestClass]
public class ServicioInicioSesionTests
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

        var service = new ServicioInicioSesion(
            repoUsuarioMock.Object,
            repoSesionMock.Object
        );

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

    [TestMethod]
    public void Login_EmailNoExiste_LanzaExcepcion()
    {
        var repoUsuarioMock = new Mock<IRepositorioUsuario>();
        var repoSesionMock = new Mock<IRepositorioSesion>();

        repoUsuarioMock
            .Setup(r => r.ObtenerPorEmail("miguel@gmail.com"))
            .Returns((Usuario?)null);

        var servicio = new ServicioInicioSesion(
            repoUsuarioMock.Object,
            repoSesionMock.Object
        );

        var request = new LoginRequest
        {
            Email = "miguel@gmail.com",
            Clave = "Password@2Ok6Abc"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() => servicio.Login(request));

        Assert.AreEqual("Credenciales inválidas", ex.Message);
        repoSesionMock.Verify(r => r.Add(It.IsAny<Sesion>()), Times.Never);
    }

    [TestMethod]
    public void Login_ClaveIncorrecta_LanzaExcepcion()
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

        var servicio = new ServicioInicioSesion(
            repoUsuarioMock.Object,
            repoSesionMock.Object
        );

        var request = new LoginRequest
        {
            Email = "miguel@gmail.com",
            Clave = "OtraClave@1aaaaaaaaaa"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() => servicio.Login(request));

        Assert.AreEqual("Credenciales inválidas", ex.Message);
        repoSesionMock.Verify(r => r.Add(It.IsAny<Sesion>()), Times.Never);
    }
}
