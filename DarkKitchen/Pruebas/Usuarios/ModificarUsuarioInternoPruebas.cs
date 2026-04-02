using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class ModificarUsuarioInternoPruebas
{
    [TestMethod]
    public void ModificarUsuarioInterno_SoloNombre_RetornaUsuarioConNombreModificado()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Nombre = "Carlos"
        };

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual(5, resultado.Id);
        Assert.AreEqual("Carlos", resultado.Nombre);
        Assert.AreEqual("Perez", resultado.Apellido);
        Assert.AreEqual("admin@gmail.com", resultado.Email);
        Assert.AreEqual("Administrativo", resultado.Rol);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void ModificarUsuarioInterno_SiSeModificaASiMismo_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Nombre = "Carlos"
        };

        service.ModificarUsuarioInterno(5, 5, request);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_NombreYApellido_RetornaUsuarioModificado()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest()
        {
            Nombre = "Carlos",
            Apellido = "Gomez"
        };

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual(5, resultado.Id);
        Assert.AreEqual("Carlos", resultado.Nombre);
        Assert.AreEqual("Gomez", resultado.Apellido);
        Assert.AreEqual("admin@gmail.com", resultado.Email);
        Assert.AreEqual("Administrativo", resultado.Rol);
    }

    [TestMethod]
    public void ModificarClaveUsuario_DatosValidos_CambiaLaClave()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuario);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarClaveRequest
        {
            ClaveActual = "PasswordSeguraee3!",
            ClaveNueva = "ClaveNuevaSegura4!"
        };

        service.ModificarClaveUsuario(5, request);

        Assert.IsTrue(usuario.VerificarClave("ClaveNuevaSegura4!"));
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void ModificarClaveUsuario_ClaveActualIncorrecta_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuario);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarClaveRequest
        {
            ClaveActual = "Incorrecta123!",
            ClaveNueva = "ClaveNuevaSegura4!"
        };

        service.ModificarClaveUsuario(5, request);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void ModificarClaveUsuario_ClaveNuevaInvalida_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuario);

        var service = new UsuarioService(repoMock.Object, Mock.Of<IValidadorTelefono>());

        var request = new ModificarClaveRequest
        {
            ClaveActual = "PasswordSeguraee3!",
            ClaveNueva = "123"
        };

        service.ModificarClaveUsuario(5, request);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void ModificarClaveUsuario_UsuarioNoExiste_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        repoMock
            .Setup(r => r.FindById(5))
            .Returns((Usuario?)null);

        var service = new UsuarioService(repoMock.Object, Mock.Of<IValidadorTelefono>());

        var request = new ModificarClaveRequest
        {
            ClaveActual = "PasswordSeguraee3!",
            ClaveNueva = "ClaveNuevaSegura4!"
        };

        service.ModificarClaveUsuario(5, request);
    }

    [TestMethod]
    public void ModificarClaveUsuario_ClaveActualIncorrecta_NoCambiaLaClave()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuario);

        var service = new UsuarioService(repoMock.Object, Mock.Of<IValidadorTelefono>());

        var request = new ModificarClaveRequest
        {
            ClaveActual = "Incorrecta123!",
            ClaveNueva = "ClaveNuevaSegura4!"
        };

        try
        {
            service.ModificarClaveUsuario(5, request);
        }
        catch (UsuarioExcepcion)
        {
        }

        Assert.IsTrue(usuario.Clave.Verificar("PasswordSeguraee3!"));
    }

    [TestMethod]
    public void ModificarUsuarioInterno_SoloApellido_RetornaUsuarioConApellidoModificado()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Apellido = "Gomez"
        };

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual("Juan", resultado.Nombre);
        Assert.AreEqual("Gomez", resultado.Apellido);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_SiNoVienenCambios_ConservaLosDatos()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest();

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual("Juan", resultado.Nombre);
        Assert.AreEqual("Perez", resultado.Apellido);
    }
}
