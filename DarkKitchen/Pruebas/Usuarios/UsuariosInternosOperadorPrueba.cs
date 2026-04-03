using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Mapeo;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class UsuariosInternosOperadorPrueba
{
    [TestMethod]
    public void CrearUsuarioInterno_DatosValidos_RetornaUsuarioResponse()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 1;

        repoMock
            .Setup(r => r.FindById(1))
            .Returns(usuarioLogueado);

        validadorTelefonoMock
            .Setup(v => v.EsValido("099123456"))
            .Returns(true);

        repoMock
            .Setup(r => r.ExisteEmail("admin@gmail.com"))
            .Returns(false);

        repoMock
            .Setup(r => r.Add(It.IsAny<Usuario>()))
            .Returns((Usuario u) =>
            {
                u.Id = 10;
                return u;
            });

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new CrearUsuarioInternoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "admin@gmail.com",
            Telefono = "099123456",
            Clave = "PasswordSeguraee3!",
            Rol = "Administrativo"
        };

        var resultado = service.CrearUsuarioInterno(1, request);

        Assert.AreEqual(10, resultado.Id);
        Assert.AreEqual("Juan", resultado.Nombre);
        Assert.AreEqual("Perez", resultado.Apellido);
        Assert.AreEqual("admin@gmail.com", resultado.Email);
        Assert.AreEqual("Administrativo", resultado.Rol);
    }

    [TestMethod]
    public void CrearUsuarioInterno_TelefonoInvalido_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 1;

        repoMock
            .Setup(r => r.FindById(1))
            .Returns(usuarioLogueado);

        validadorTelefonoMock
            .Setup(v => v.EsValido("123"))
            .Returns(false);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new CrearUsuarioInternoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "admin@gmail.com",
            Telefono = "123",
            Clave = "PasswordSeguraee3!",
            Rol = "Administrativo"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.CrearUsuarioInterno(1, request)
        );

        Assert.AreEqual("El teléfono no es válido", ex.Message);
    }

    [TestMethod]
    public void CrearUsuarioInterno_NombreConNumeros_LanzaExcepcion()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Usuario.CrearInterno(
                "Ju4n",
                "Perez",
                "admin@gmail.com",
                Telefono.Crear("099123456"),
                "PasswordSeguraee3!",
                RolUsuario.Administrativo
            )
        );

        Assert.AreEqual("El nombre no puede contener números", ex.Message);
    }

    [TestMethod]
    public void CrearUsuarioInterno_TelefonoInvalido_LanzaExcepcionYNoLlamaAlRepositorio()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 1;

        repoMock
            .Setup(r => r.FindById(1))
            .Returns(usuarioLogueado);

        validadorTelefonoMock
            .Setup(v => v.EsValido("123"))
            .Returns(false);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new CrearUsuarioInternoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "admin@gmail.com",
            Telefono = "123",
            Clave = "PasswordSeguraee3!",
            Rol = "Administrativo"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.CrearUsuarioInterno(1, request)
        );

        Assert.AreEqual("El teléfono no es válido", ex.Message);

        repoMock.Verify(r => r.Add(It.IsAny<Usuario>()), Times.Never);
    }

    [TestMethod]
    public void CrearUsuarioInterno_DatosValidos_EntraAlRepositorioUnaVez()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 1;

        repoMock
            .Setup(r => r.FindById(1))
            .Returns(usuarioLogueado);

        validadorTelefonoMock
            .Setup(v => v.EsValido("099123456"))
            .Returns(true);

        repoMock
            .Setup(r => r.ExisteEmail("admin@gmail.com"))
            .Returns(false);

        repoMock
            .Setup(r => r.Add(It.IsAny<Usuario>()))
            .Returns((Usuario u) =>
            {
                u.Id = 10;
                return u;
            });

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new CrearUsuarioInternoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "admin@gmail.com",
            Telefono = "099123456",
            Clave = "PasswordSeguraee3!",
            Rol = "Administrativo"
        };

        service.CrearUsuarioInterno(1, request);

        repoMock.Verify(r => r.Add(It.IsAny<Usuario>()), Times.Once);
    }

    [TestMethod]
    public void CrearUsuarioInterno_EmailYaRegistrado_NoLlamaAdd()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 1;

        repoMock
            .Setup(r => r.FindById(1))
            .Returns(usuarioLogueado);

        validadorTelefonoMock
            .Setup(v => v.EsValido("099123456"))
            .Returns(true);

        repoMock
            .Setup(r => r.ExisteEmail("admin@gmail.com"))
            .Returns(true);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new CrearUsuarioInternoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "admin@gmail.com",
            Telefono = "099123456",
            Clave = "PasswordSeguraee3!",
            Rol = "AdmiNistrAtIvo"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.CrearUsuarioInterno(1, request)
        );

        Assert.AreEqual("El email ya está registrado", ex.Message);

        repoMock.Verify(r => r.Add(It.IsAny<Usuario>()), Times.Never);
    }

    [TestMethod]
    public void MapeoDTOAUserInterno_DatosValidos_RetornaUsuarioInterno()
    {
        var request = new CrearUsuarioInternoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "admin@gmail.com",
            Telefono = "099123456",
            Clave = "PasswordSeguraee3!",
            Rol = "Administrativo"
        };

        var telefono = Telefono.Crear("099123456");
        var rol = RolUsuario.Administrativo;

        var usuario = request.MapeoDTOAUserInterno(telefono, rol);

        Assert.AreEqual("Juan", usuario.Nombre);
        Assert.AreEqual("Perez", usuario.Apellido);
        Assert.AreEqual("admin@gmail.com", usuario.Email.ToString());
        Assert.AreEqual("099123456", usuario.Telefono.Valor);
        Assert.AreEqual(RolUsuario.Administrativo, usuario.Rol);
    }

    [TestMethod]
    public void MapeoAResponseDTO_UsuarioValido_MapeaCorrectamente()
    {
        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );

        usuario.Id = 10;

        var response = usuario.MapeoAResponseDTO();

        Assert.AreEqual(10, response.Id);
        Assert.AreEqual("Juan", response.Nombre);
        Assert.AreEqual("Perez", response.Apellido);
        Assert.AreEqual("admin@gmail.com", response.Email);
        Assert.AreEqual("Administrativo", response.Rol);
    }

    [TestMethod]
    public void MapeoAResponseDTO_UsuarioNull_LanzaExcepcionControlada()
    {
        Usuario usuario = null!;

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            usuario.MapeoAResponseDTO()
        );

        Assert.AreEqual("El usuario no puede ser null", ex.Message);
    }

    [TestMethod]
    public void CrearUsuarioInterno_RolInvalido_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var admin = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        admin.Id = 1;

        repoMock
            .Setup(r => r.FindById(1))
            .Returns(admin);

        validadorTelefonoMock
            .Setup(v => v.EsValido("099123456"))
            .Returns(true);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new CrearUsuarioInternoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "admin@gmail.com",
            Telefono = "099123456",
            Clave = "PasswordSeguraee3!",
            Rol = "CualquierCosa"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.CrearUsuarioInterno(1, request)
        );

        Assert.AreEqual("El rol interno no es válido", ex.Message);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearInterno_RolCliente_LanzaExcepcion()
    {
        Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Cliente
        );
    }

    [TestMethod]
    public void CrearInterno_NombreVacio_LanzaExcepcion()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Usuario.CrearInterno(
                string.Empty,
                "Perez",
                "admin@gmail.com",
                Telefono.Crear("099123456"),
                "PasswordSeguraee3!",
                RolUsuario.Administrativo
            )
        );

        Assert.AreEqual("El nombre no puede estar vacío", ex.Message);
    }

    [TestMethod]
    public void CrearInterno_ApellidoVacio_LanzaExcepcion()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Usuario.CrearInterno(
                "Juan",
                string.Empty,
                "admin@gmail.com",
                Telefono.Crear("099123456"),
                "PasswordSeguraee3!",
                RolUsuario.Administrativo
            )
        );

        Assert.AreEqual("El apellido no puede estar vacío", ex.Message);
    }

    [TestMethod]
    public void CrearInterno_ApellidoMenorATres_LanzaExcepcion()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Usuario.CrearInterno(
                "Juan",
                "Pe",
                "admin@gmail.com",
                Telefono.Crear("099123456"),
                "PasswordSeguraee3!",
                RolUsuario.Administrativo
            )
        );

        Assert.AreEqual("El apellido debe tener entre 3 y 25 caracteres", ex.Message);
    }

    [TestMethod]
    public void CrearInterno_ApellidoMayorAVeinticinco_LanzaExcepcion()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Usuario.CrearInterno(
                "Juan",
                "ApellidoDemasiadoDemasiadoLargo",
                "admin@gmail.com",
                Telefono.Crear("099123456"),
                "PasswordSeguraee3!",
                RolUsuario.Administrativo
            )
        );

        Assert.AreEqual("El apellido debe tener entre 3 y 25 caracteres", ex.Message);
    }

    [TestMethod]
    public void CrearTelefono_Vacio_LanzaExcepcion()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Telefono.Crear(string.Empty)
        );

        Assert.AreEqual("El teléfono no puede estar vacío", ex.Message);
    }

    [TestMethod]
    public void CrearUsuarioInterno_DatosValidos_LlamaExisteEmailUnaVez()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 1;

        repoMock
            .Setup(r => r.FindById(1))
            .Returns(usuarioLogueado);

        validadorTelefonoMock
            .Setup(v => v.EsValido("099123456"))
            .Returns(true);

        repoMock
            .Setup(r => r.ExisteEmail("admin@gmail.com"))
            .Returns(false);

        repoMock
            .Setup(r => r.Add(It.IsAny<Usuario>()))
            .Returns((Usuario u) =>
            {
                u.Id = 10;
                return u;
            });

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new CrearUsuarioInternoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "admin@gmail.com",
            Telefono = "099123456",
            Clave = "PasswordSeguraee3!",
            Rol = "Administrativo"
        };

        service.CrearUsuarioInterno(1, request);

        repoMock.Verify(r => r.ExisteEmail("admin@gmail.com"), Times.Once);
    }

    [TestMethod]
    public void CrearUsuarioInterno_DatosValidos_ValidaTelefonoUnaVez()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 1;

        repoMock
            .Setup(r => r.FindById(1))
            .Returns(usuarioLogueado);

        validadorTelefonoMock
            .Setup(v => v.EsValido("099123456"))
            .Returns(true);

        repoMock
            .Setup(r => r.ExisteEmail("admin@gmail.com"))
            .Returns(false);

        repoMock
            .Setup(r => r.Add(It.IsAny<Usuario>()))
            .Returns((Usuario u) =>
            {
                u.Id = 10;
                return u;
            });

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new CrearUsuarioInternoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "admin@gmail.com",
            Telefono = "099123456",
            Clave = "PasswordSeguraee3!",
            Rol = "Administrativo"
        };

        service.CrearUsuarioInterno(1, request);

        validadorTelefonoMock.Verify(v => v.EsValido("099123456"), Times.Once);
    }

    [TestMethod]
    public void CrearUsuarioInterno_UsuarioNoAdministrativo_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioLogueado = Usuario.CrearCliente(
            "Juan",
            "Perez",
            "cliente@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!"
        );
        usuarioLogueado.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioLogueado);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new CrearUsuarioInternoRequest();

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.CrearUsuarioInterno(5, request)
        );

        Assert.AreEqual("Solo un usuario administrativo puede realizar esta acción", ex.Message); // ajustá el mensaje según tu implementación
    }

    [TestMethod]
    public void CrearUsuarioInterno_UsuarioLogueadoNoExiste_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        repoMock
            .Setup(r => r.FindById(1))
            .Returns((Usuario?)null);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new CrearUsuarioInternoRequest();

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.CrearUsuarioInterno(1, request)
        );

        Assert.AreEqual("El usuario no existe", ex.Message);
    }
}
