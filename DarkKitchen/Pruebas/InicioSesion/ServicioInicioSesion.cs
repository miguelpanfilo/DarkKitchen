using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
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
        var relojMock = new Mock<IReloj>();

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
            repoSesionMock.Object,
            relojMock.Object
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
        var relojMock = new Mock<IReloj>();

        repoUsuarioMock
            .Setup(r => r.ObtenerPorEmail("miguel@gmail.com"))
            .Returns((Usuario?)null);

        var servicio = new ServicioInicioSesion(
            repoUsuarioMock.Object,
            repoSesionMock.Object,
            relojMock.Object
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
        var relojMock = new Mock<IReloj>();

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
            repoSesionMock.Object,
            relojMock.Object
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

    [TestMethod]
    public void Login_CredencialesValidas_GuardaSesion()
    {
        var repoUsuarioMock = new Mock<IRepositorioUsuario>();
        var repoSesionMock = new Mock<IRepositorioSesion>();
        var relojMock = new Mock<IReloj>();

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

        var servicio = new ServicioInicioSesion(
            repoUsuarioMock.Object,
            repoSesionMock.Object,
            relojMock.Object
        );

        var request = new LoginRequest
        {
            Email = "miguel@gmail.com",
            Clave = "Password@2Ok6Abc"
        };

        servicio.Login(request);

        repoSesionMock.Verify(r => r.Add(It.IsAny<Sesion>()), Times.Once);
    }

    [TestMethod]
    public void Login_ClienteValido_RetornaRolCliente()
    {
        var repoUsuarioMock = new Mock<IRepositorioUsuario>();
        var repoSesionMock = new Mock<IRepositorioSesion>();
        var relojMock = new Mock<IReloj>();

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

        var servicio = new ServicioInicioSesion(
            repoUsuarioMock.Object,
            repoSesionMock.Object,
            relojMock.Object
        );

        var request = new LoginRequest
        {
            Email = "miguel@gmail.com",
            Clave = "Password@2Ok6Abc"
        };

        var response = servicio.Login(request);

        Assert.AreEqual("Cliente", response.Rol);
    }

    [TestMethod]
    public void Login_CredencialesValidas_RetornaUsuarioIdCorrecto()
    {
        var repoUsuarioMock = new Mock<IRepositorioUsuario>();
        var repoSesionMock = new Mock<IRepositorioSesion>();
        var relojMock = new Mock<IReloj>();

        var usuario = Usuario.CrearCliente(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2Ok6Abc"
        );
        usuario.Id = 25;

        repoUsuarioMock
            .Setup(r => r.ObtenerPorEmail("miguel@gmail.com"))
            .Returns(usuario);

        repoSesionMock
            .Setup(r => r.Add(It.IsAny<Sesion>()))
            .Returns((Sesion s) => s);

        var servicio = new ServicioInicioSesion(
            repoUsuarioMock.Object,
            repoSesionMock.Object,
            relojMock.Object
        );

        var request = new LoginRequest
        {
            Email = "miguel@gmail.com",
            Clave = "Password@2Ok6Abc"
        };

        var response = servicio.Login(request);

        Assert.AreEqual(25, response.UsuarioId);
    }

    [TestMethod]
    public void Login_CredencialesValidas_GeneraSessionIdNoVacio()
    {
        var repoUsuarioMock = new Mock<IRepositorioUsuario>();
        var repoSesionMock = new Mock<IRepositorioSesion>();
        var relojMock = new Mock<IReloj>();

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

        var servicio = new ServicioInicioSesion(
            repoUsuarioMock.Object,
            repoSesionMock.Object,
            relojMock.Object
        );

        var request = new LoginRequest
        {
            Email = "miguel@gmail.com",
            Clave = "Password@2Ok6Abc"
        };

        var response = servicio.Login(request);

        Assert.AreNotEqual(Guid.Empty, response.SessionId);
    }

    [TestMethod]
    public void Login_ClaveIncorrecta_NoGuardaSesion()
    {
        var repoUsuarioMock = new Mock<IRepositorioUsuario>();
        var repoSesionMock = new Mock<IRepositorioSesion>();
        var relojMock = new Mock<IReloj>();

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
            repoSesionMock.Object,
            relojMock.Object
        );

        var request = new LoginRequest
        {
            Email = "miguel@gmail.com",
            Clave = "Incorrecta@123456"
        };

        Assert.ThrowsException<UsuarioExcepcion>(() => servicio.Login(request));

        repoSesionMock.Verify(r => r.Add(It.IsAny<Sesion>()), Times.Never);
    }

    [TestMethod]
    public void Login_EmailNoExiste_NoGuardaSesion()
    {
        var repoUsuarioMock = new Mock<IRepositorioUsuario>();
        var repoSesionMock = new Mock<IRepositorioSesion>();
        var relojMock = new Mock<IReloj>();

        repoUsuarioMock
            .Setup(r => r.ObtenerPorEmail("miguel@gmail.com"))
            .Returns((Usuario?)null);

        var servicio = new ServicioInicioSesion(
            repoUsuarioMock.Object,
            repoSesionMock.Object,
            relojMock.Object
        );

        var request = new LoginRequest
        {
            Email = "miguel@gmail.com",
            Clave = "Password@2Ok6Abc"
        };

        Assert.ThrowsException<UsuarioExcepcion>(() => servicio.Login(request));

        repoSesionMock.Verify(r => r.Add(It.IsAny<Sesion>()), Times.Never);
    }

    [TestMethod]
    public void Login_CredencialesValidas_RetornaExpiracionCorrecta()
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

        var relojMock = new Mock<IReloj>();

        var fechaFija = new DateTime(2026, 1, 1, 10, 0, 0);

        relojMock
            .Setup(r => r.Ahora())
            .Returns(fechaFija);

        var servicio = new ServicioInicioSesion(
            repoUsuarioMock.Object,
            repoSesionMock.Object,
            relojMock.Object
        );

        var request = new LoginRequest
        {
            Email = "miguel@gmail.com",
            Clave = "Password@2Ok6Abc"
        };

        var response = servicio.Login(request);

        var expiracionEsperada = fechaFija.AddHours(2);

        Assert.AreEqual(expiracionEsperada, response.Expiracion);
    }

    [TestMethod]
    public void Login_EmailConMayusculasYMinusculas_RetornaSesion()
    {
        var repoUsuarioMock = new Mock<IRepositorioUsuario>();
        var repoSesionMock = new Mock<IRepositorioSesion>();
        var relojMock = new Mock<IReloj>();

        var usuario = Usuario.CrearCliente(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2Ok6Abc"
        );
        usuario.Id = 1;

        repoUsuarioMock
            .Setup(r => r.ObtenerPorEmail("MIGUEL@GMAIL.COM"))
            .Returns(usuario);

        repoSesionMock
            .Setup(r => r.Add(It.IsAny<Sesion>()))
            .Returns((Sesion s) => s);

        var servicio = new ServicioInicioSesion(
            repoUsuarioMock.Object,
            repoSesionMock.Object,
            relojMock.Object
        );

        var request = new LoginRequest
        {
            Email = "MIGUEL@GMAIL.COM",
            Clave = "Password@2Ok6Abc"
        };

        var response = servicio.Login(request);

        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.UsuarioId);
    }

    [TestMethod]
    public void Login_CredencialesValidas_GuardaSesionConLaMismaExpiracionQueLaRespuesta()
    {
        var repoUsuarioMock = new Mock<IRepositorioUsuario>();
        var repoSesionMock = new Mock<IRepositorioSesion>();
        var relojMock = new Mock<IReloj>();

        var fechaFija = new DateTime(2026, 1, 1, 10, 0, 0);

        relojMock
            .Setup(r => r.Ahora())
            .Returns(fechaFija);

        var usuario = Usuario.CrearCliente(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2Ok6Abc"
        );
        usuario.Id = 1;

        Sesion? sesionGuardada = null;

        repoUsuarioMock
            .Setup(r => r.ObtenerPorEmail("miguel@gmail.com"))
            .Returns(usuario);

        repoSesionMock
            .Setup(r => r.Add(It.IsAny<Sesion>()))
            .Callback<Sesion>(s => sesionGuardada = s)
            .Returns((Sesion s) => s);

        var servicio = new ServicioInicioSesion(
            repoUsuarioMock.Object,
            repoSesionMock.Object,
            relojMock.Object
        );

        var request = new LoginRequest
        {
            Email = "miguel@gmail.com",
            Clave = "Password@2Ok6Abc"
        };

        var response = servicio.Login(request);

        Assert.IsNotNull(sesionGuardada);
        Assert.AreEqual(sesionGuardada.Expiracion, response.Expiracion);
    }
}
