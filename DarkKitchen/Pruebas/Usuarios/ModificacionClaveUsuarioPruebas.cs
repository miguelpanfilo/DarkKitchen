using System.Security;
using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class ModificacionClaveUsuarioPruebas
{
    [TestMethod]
    public void ModificarClaveUsuario_UsuarioLogueadoNoExiste_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var service = new ModificacionUsuarioService(repoMock.Object, validadorTelefonoMock.Object);
        var usuarioAModificar = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioAModificar.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioAModificar);

        repoMock
            .Setup(r => r.FindById(99))
            .Returns((Usuario?)null);

        var request = new ModificarClaveRequest
        {
            ClaveActual = "PasswordSeguraee3!",
            ClaveNueva = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarClaveUsuario(5, 99, request)
        );

        Assert.AreEqual("El usuario no existe", ex.Message);
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
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuario);

        var usuarioLogueado = Usuario.CrearInterno(
            "Juan Martin",
            "Perez",
            "admin2@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguyyyy3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new ModificacionUsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarClaveRequest
        {
            ClaveActual = "PasswordSeguraee3!", ClaveNueva = "ClaveNuevaSegura4!"
        };

        service.ModificarClaveUsuario(5, 99,request);

        Assert.IsTrue(usuario.VerificarClave("ClaveNuevaSegura4!"));
    }

    [TestMethod]
    public void ModificarClaveUsuario_UsuarioLogueadoNoEsAdministrativo_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioAModificar = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioAModificar.Id = 5;

        var usuarioLogueado = Usuario.CrearCliente(
            "Pedro",
            "Gomez",
            "cliente@gmail.com",
            Telefono.Crear("099111222"),
            "PasswordSeguraee3!"
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioAModificar);

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new ModificacionUsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarClaveRequest
        {
            ClaveActual = "PasswordSeguraee3!",
            ClaveNueva = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarClaveUsuario(5, 99, request)
        );

        Assert.AreEqual("Solo un usuario administrativo puede realizar esta acción", ex.Message);
    }

    [TestMethod]
    public void ModificarClaveUsuario_ClaveActualIncorrecta_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 5;

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock.Setup(r => r.FindById(5)).Returns(usuario);
        repoMock.Setup(r => r.FindById(99)).Returns(usuarioLogueado);

        var service = new ModificacionUsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarClaveRequest
        {
            ClaveActual = "Incorrecta123!",
            ClaveNueva = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarClaveUsuario(5, 99, request)
        );

        Assert.AreEqual("La clave actual no es correcta", ex.Message);
    }

    [TestMethod]
    public void ModificarClaveUsuario_ClaveNuevaInvalida_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuario);

        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new ModificacionUsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarClaveRequest
        {
            ClaveActual = "PasswordSeguraee3!",
            ClaveNueva = "123"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarClaveUsuario(5, 99, request)
        );

        Assert.AreEqual("La clave no tiene el formato válido", ex.Message);
    }

    [TestMethod]
    public void ModificarClaveUsuario_UsuarioNoExiste_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        repoMock
            .Setup(r => r.FindById(5))
            .Returns((Usuario?)null);

        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new ModificacionUsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarClaveRequest
        {
            ClaveActual = "PasswordSeguraee3!",
            ClaveNueva = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarClaveUsuario(5, 99, request)
        );

        Assert.AreEqual("El usuario no existe", ex.Message);
    }

    [TestMethod]
    public void ModificarClaveUsuario_ClaveActualIncorrecta_NoCambiaLaClave()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuario);

        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new ModificacionUsuarioService(repoMock.Object, validadorTelefonoMock.Object);
        var request = new ModificarClaveRequest
        {
            ClaveActual = "Incorrecta123!",
            ClaveNueva = "ClaveNuevaSegura4!"
        };

        Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarClaveUsuario(5, 99, request)
        );

        Assert.IsTrue(usuario.Clave.Verificar("PasswordSeguraee3!"));
    }
}
