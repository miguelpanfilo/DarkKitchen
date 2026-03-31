using Almacenamiento;
using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class UsuarioServiceTests
{
    [TestMethod]
    public void RegistrarCliente_DatosValidos_RetornaUsuario()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        repoMock.Setup(r => r.Add(It.IsAny<Usuario>()))
            .Returns((Usuario u) => u);

        validadorTelefonoMock
            .Setup(v => v.EsValido(It.IsAny<string>()))
            .Returns(true);

        var usuarioServicio = new UsuarioService(
            repoMock.Object,
            validadorTelefonoMock.Object);

        var solicitudDeRegistro = new RegistrarClienteRequest
        {
            Nombre = "Miguel",
            Apellido = "Lopez",
            Email = "miguel@gmail.com",
            Telefono = "099123456",
            Clave = "Password@2026Ok"
        };

        var resultado = usuarioServicio.RegistrarCliente(solicitudDeRegistro);

        Assert.IsNotNull(resultado);
        Assert.AreEqual("Miguel", resultado.Nombre);
        Assert.AreEqual("Lopez", resultado.Apellido);
        Assert.AreEqual("miguel@gmail.com", resultado.Email);
    }

    [TestMethod]
    public void RegistrarCliente_UsuarioValido_EnMemoria()
    {
        var repositorioUsuario = new RepositorioUsuario();

        var usuario = Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2026Ok");

        repositorioUsuario.Add(usuario);

        var usuarios = repositorioUsuario.GetAll();

        Assert.AreEqual(1, usuarios.Count);
        Assert.AreEqual("Miguel", usuarios[0].Nombre);
        Assert.AreEqual("Lopez", usuarios[0].Apellido);
        Assert.AreEqual("miguel@gmail.com", usuarios[0].Email);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void RegistrarCliente_SinNombre_LanzaExcepcion()
    {
        var usuario = Usuario.Crear(
            string.Empty,
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2026Ok");
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void RegistrarCliente_SinApellido_LanzaExcepcion()
    {
        var usuario = Usuario.Crear(
            "Miguel",
            string.Empty,
            "miguel@gmail.com",
            "099123456",
            "Password@2026Ok");
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void RegistrarCliente_ConApellidoMas25_LanzaExcepcion()
    {
        var largoIncorrectoDeApellido = 26;
        var apellidoLargo = new string('a', largoIncorrectoDeApellido);

        var usuario = Usuario.Crear(
            "Miguel",
            apellidoLargo,
            "miguel@gmail.com",
            "099123456",
            "Password@2026Ok");
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void RegistrarCliente_ConApellidoMenosDe3_LanzaExcepcion()
    {
        var largoCortoDeApellido = 2;
        var apellidoCorto = new string('a', largoCortoDeApellido);

        var usuario = Usuario.Crear(
            "Miguel",
            apellidoCorto,
            "miguel@gmail.com",
            "099123456",
            "Password@2026Ok");
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void RegistrarCliente_EmailVacio_LanzaExcepcion()
    {
        var usuario = Usuario.Crear(
            "Miguel",
            "Lopez",
            string.Empty,
            "099123456",
            "Password@2026Ok"
        );
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void RegistrarCliente_EmailSinArroba_LanzaExcepcion()
    {
        var usuario = Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguelgmail.com",
            "099123456",
            "Password@2026Ok"
        );
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void RegistrarCliente_EmailSinDominioValido_LanzaExcepcion()
    {
        var usuario = Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail",
            "099123456",
            "Password@2026Ok"
        );
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void RegistrarCliente_TelefonoVacio_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        validadorTelefonoMock
            .Setup(v => v.EsValido(It.IsAny<string>()))
            .Returns(false);

        var usuarioServicio = new UsuarioService(
            repoMock.Object,
            validadorTelefonoMock.Object
        );

        var solicitudDeRegistro = new RegistrarClienteRequest
        {
            Nombre = "Miguel",
            Apellido = "Lopez",
            Email = "miguel@gmail.com",
            Telefono = string.Empty,
            Clave = "Password@2026Ok"
        };

        usuarioServicio.RegistrarCliente(solicitudDeRegistro);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void RegistrarCliente_TelefonoInvalido_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        validadorTelefonoMock
            .Setup(v => v.EsValido(It.IsAny<string>()))
            .Returns(false);

        var usuarioServicio = new UsuarioService(
            repoMock.Object,
            validadorTelefonoMock.Object
        );

        var solicitudDeRegistro = new RegistrarClienteRequest
        {
            Nombre = "Miguel",
            Apellido = "Lopez",
            Email = "miguel@gmail.com",
            Telefono = "123",
            Clave = "Password@2026Ok"
        };

        usuarioServicio.RegistrarCliente(solicitudDeRegistro);
    }
}
