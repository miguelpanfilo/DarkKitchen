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
    public void CrearUsuario_ApellidoMaximoValido_NoLanzaExcepcion()
    {
        var apellido = new string('a', 25);

        var usuario = Usuario.Crear(
            "Miguel",
            apellido,
            "miguel@gmail.com",
            "099123456",
            "Password!2026Ok"
        );

        Assert.IsNotNull(usuario);
    }

    [TestMethod]
    public void CrearUsuario_ApellidoMinimoValido_NoLanzaExcepcion()
    {
        var usuario = Usuario.Crear(
            "Miguel",
            "Lop",
            "miguel@gmail.com",
            "099123456",
            "Password!2026Ok"
        );

        Assert.IsNotNull(usuario);
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

    [TestMethod]
    public void RegistrarCliente_TelefonoValidoUruguay_RegistroExitoso()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        validadorTelefonoMock
            .Setup(v => v.EsValido(It.IsAny<string>()))
            .Returns(true);

        repoMock.Setup(r => r.Add(It.IsAny<Usuario>()))
            .Returns((Usuario u) => u);

        var usuarioServicio = new UsuarioService(
            repoMock.Object,
            validadorTelefonoMock.Object
        );

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
    }

    [TestMethod]
    public void ExisteEmail_EmailYaExiste_RetornaTrue()
    {
        var repo = new RepositorioUsuario();

        var usuario = Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2026Ok"
        );

        repo.Add(usuario);

        var existe = repo.ExisteEmail("MIGUEL@gmail.com");

        Assert.IsTrue(existe);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void RegistrarCliente_EmailDuplicado_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        repoMock.Setup(r => r.ExisteEmail("miguel@gmail.com"))
            .Returns(true);

        validadorTelefonoMock
            .Setup(v => v.EsValido(It.IsAny<string>()))
            .Returns(true);

        var usuarioServicio = new UsuarioService(
            repoMock.Object,
            validadorTelefonoMock.Object
        );

        var solicitudDeRegistro = new RegistrarClienteRequest
        {
            Nombre = "Miguel",
            Apellido = "Lopez",
            Email = "miguel@gmail.com",
            Telefono = "099123456",
            Clave = "Password@2026Ok"
        };

        usuarioServicio.RegistrarCliente(solicitudDeRegistro);
    }

    [TestMethod]
    public void Add_UsuarioNuevo_SeAsignaIdIncremental()
    {
        var repo = new RepositorioUsuario();

        var usuario1 = Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2026Ok"
        );

        var usuario2 = Usuario.Crear(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099999999",
            "Password@2026Ok"
        );

        var userAgregadoPrimero = repo.Add(usuario1);
        var userAgregadoSegundo = repo.Add(usuario2);

        var primerUsuarioAgregado = 1;
        var segundoUsuarioAgregado = 2;

        Assert.AreEqual(primerUsuarioAgregado, userAgregadoPrimero.Id);
        Assert.AreEqual(segundoUsuarioAgregado, userAgregadoSegundo.Id);
    }

    [TestMethod]
    public void Add_Usuario_SeAsignaIdMayorACero()
    {
        var repo = new RepositorioUsuario();

        var usuario = Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2026Ok"
        );

        var resultado = repo.Add(usuario);

        Assert.IsTrue(resultado.Id > 0);
    }

    [TestMethod]
    public void CrearUsuario_EmailSeNormaliza_Correctamente()
    {
        var usuario = Usuario.Crear(
            "Miguel",
            "Lopez",
            "  MIGUEL@gmail.com  ",
            "099123456",
            "Password!2026Ok"
        );

        Assert.AreEqual("miguel@gmail.com", usuario.Email.ToString());
    }

    [TestMethod]
    public void RegistrarCliente_EmailNoDuplicado_RegistroExitoso()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        repoMock.Setup(r => r.ExisteEmail(It.IsAny<string>()))
            .Returns(false);

        repoMock.Setup(r => r.Add(It.IsAny<Usuario>()))
            .Returns((Usuario u) => u);

        validadorTelefonoMock
            .Setup(v => v.EsValido(It.IsAny<string>()))
            .Returns(true);

        var usuarioServicio = new UsuarioService(
            repoMock.Object,
            validadorTelefonoMock.Object
        );

        var solicitud = new RegistrarClienteRequest
        {
            Nombre = "Miguel",
            Apellido = "Lopez",
            Email = "miguel@gmail.com",
            Telefono = "099123456",
            Clave = "Password!2026Ok"
        };

        var resultado = usuarioServicio.RegistrarCliente(solicitud);

        Assert.IsNotNull(resultado);
        Assert.AreEqual("miguel@gmail.com", resultado.Email);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearUsuario_ClaveVacia_LanzaExcepcion()
    {
        Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            string.Empty
        );
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearUsuario_ClaveMenorA15_LanzaExcepcion()
    {
        var palabraDeLargo13 = 13;
        var claveMenor15Caracteres = new string('a', palabraDeLargo13);

        Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            claveMenor15Caracteres);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearUsuario_ClaveMayorA25_LanzaExcepcion()
    {
        var palabraDeLargo26 = 26;
        var claveMayorA25Caracteres = new string('a', palabraDeLargo26);

        Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            claveMayorA25Caracteres);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearUsuario_ClaveSinMayuscula_LanzaExcepcion()
    {
        Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "password!abc9xy");
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearUsuario_ClaveSinMinuscula_LanzaExcepcion()
    {
        Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "PASSWORD!ABC9XY");
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearUsuario_ClaveSinNumero_LanzaExcepcion()
    {
        Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password!AbcXyz");
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearUsuario_ClaveSinSimbolo_LanzaExcepcion()
    {
        Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "PasswordAbc123Xy");
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearUsuario_ClaveConSecuenciaDeNumeros_LanzaExcepcion()
    {
        Usuario.Crear(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password!Ab12Xy");
    }
}
