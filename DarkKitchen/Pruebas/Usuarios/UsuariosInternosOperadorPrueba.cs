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

        var resultado = service.CrearUsuarioInterno(request);

        Assert.AreEqual(10, resultado.Id);
        Assert.AreEqual("Juan", resultado.Nombre);
        Assert.AreEqual("Perez", resultado.Apellido);
        Assert.AreEqual("admin@gmail.com", resultado.Email);
        Assert.AreEqual("Administrativo", resultado.Rol);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearUsuarioInterno_TelefonoInvalido_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

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

        service.CrearUsuarioInterno(request);
    }

    [TestMethod]
    public void CrearUsuarioInterno_TelefonoInvalido_NoLlamaAdd()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

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

        try
        {
            service.CrearUsuarioInterno(request);
        }
        catch (UsuarioExcepcion)
        {
        }

        repoMock.Verify(r => r.Add(It.IsAny<Usuario>()), Times.Never);
    }

    [TestMethod]
    public void CrearUsuarioInterno_DatosValidos_LlamaAddUnaVez()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

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

        service.CrearUsuarioInterno(request);

        repoMock.Verify(r => r.Add(It.IsAny<Usuario>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearUsuarioInterno_EmailYaRegistrado_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

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
            Rol = "Administrativo"
        };

        service.CrearUsuarioInterno(request);
    }

    [TestMethod]
    public void MapeoDTOAUserInterno_RolEnMinuscula_MapeaCorrectamente()
    {
        var request = new CrearUsuarioInternoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "admin@gmail.com",
            Telefono = "099123456",
            Clave = "PasswordSeguraee3!",
            Rol = "administrativo"
        };

        var usuario = request.MapeoDTOAUserInterno();

        Assert.AreEqual(RolUsuario.Administrativo, usuario.Rol);
    }

    [TestMethod]
    public void CrearUsuarioInterno_EmailYaRegistrado_NoLlamaAdd()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

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

        try
        {
            service.CrearUsuarioInterno(request);
        }
        catch (UsuarioExcepcion)
        {
        }

        repoMock.Verify(r => r.Add(It.IsAny<Usuario>()), Times.Never);
    }

    [TestMethod]
    public void MapeoDTOAUserInterno_DatosValidos_MapeaCorrectamente()
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

        var usuario = request.MapeoDTOAUserInterno();

        Assert.AreEqual("Juan", usuario.Nombre);
        Assert.AreEqual("Perez", usuario.Apellido);
        Assert.AreEqual("admin@gmail.com", usuario.Email.ToString());
        Assert.AreEqual(RolUsuario.Administrativo, usuario.Rol);
    }

    [TestMethod]
    public void MapeoAResponseDTO_UsuarioValido_MapeaCorrectamente()
    {
        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
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
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void MapeoAResponseDTO_UsuarioNull_LanzaExcepcionControlada()
    {
        Usuario usuario = null!;

        usuario.MapeoAResponseDTO();
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void MapeoDTOAUserInterno_RolInvalido_LanzaExcepcion()
    {
        var request = new CrearUsuarioInternoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "admin@gmail.com",
            Telefono = "099123456",
            Clave = "PasswordSeguraee3!",
            Rol = "CualquierCosa"
        };

        request.MapeoDTOAUserInterno();
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearInterno_RolCliente_LanzaExcepcion()
    {
        Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Cliente
        );
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearInterno_NombreVacio_LanzaExcepcion()
    {
        Usuario.CrearInterno(
            string.Empty,
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearInterno_ApellidoVacio_LanzaExcepcion()
    {
        Usuario.CrearInterno(
            "Juan",
            String.Empty,
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearInterno_ApellidoMenorATres_LanzaExcepcion()
    {
        Usuario.CrearInterno(
            "Juan",
            "Pe",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearInterno_ApellidoMayorAVeinticinco_LanzaExcepcion()
    {
        Usuario.CrearInterno(
            "Juan",
            "ApellidoDemasiadoDemasiadoLargo",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearInterno_TelefonoVacio_LanzaExcepcion()
    {
        Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            string.Empty,
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
    }

    [TestMethod]
    public void CrearUsuarioInterno_DatosValidos_LlamaExisteEmailUnaVez()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

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

        service.CrearUsuarioInterno(request);

        repoMock.Verify(r => r.ExisteEmail("admin@gmail.com"), Times.Once);
    }

    [TestMethod]
    public void CrearUsuarioInterno_DatosValidos_ValidaTelefonoUnaVez()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

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

        service.CrearUsuarioInterno(request);

        validadorTelefonoMock.Verify(v => v.EsValido("099123456"), Times.Once);
    }
}
