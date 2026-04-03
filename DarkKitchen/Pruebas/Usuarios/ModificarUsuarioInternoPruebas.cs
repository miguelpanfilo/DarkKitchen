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
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Nombre = "Carlos" };

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual(5, resultado.Id);
        Assert.AreEqual("Carlos", resultado.Nombre);
        Assert.AreEqual("Perez", resultado.Apellido);
        Assert.AreEqual("admin@gmail.com", resultado.Email);
        Assert.AreEqual("Administrativo", resultado.Rol);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_SiSeModificaASiMismo_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Nombre = "Carlos" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarUsuarioInterno(5, 5, request)
        );

        Assert.AreEqual("Un usuario no puede modificarse a sí mismo", ex.Message);
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
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;
        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin2@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;
        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest() { Nombre = "Carlos", Apellido = "Gomez" };

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual(5, resultado.Id);
        Assert.AreEqual("Carlos", resultado.Nombre);
        Assert.AreEqual("Gomez", resultado.Apellido);
        Assert.AreEqual("admin@gmail.com", resultado.Email);
        Assert.AreEqual("Administrativo", resultado.Rol);
    }

    [TestMethod]
    public void ModificarClaveUsuario_UsuarioLogueadoNoExiste_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var service = new UsuarioService(repoMock.Object, Mock.Of<IValidadorTelefono>());

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

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

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

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

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

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

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

        var service = new UsuarioService(repoMock.Object, Mock.Of<IValidadorTelefono>());

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

        var service = new UsuarioService(repoMock.Object, Mock.Of<IValidadorTelefono>());

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

        var service = new UsuarioService(repoMock.Object, Mock.Of<IValidadorTelefono>());

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

    [TestMethod]
    public void ModificarUsuarioInterno_SoloApellido_RetornaUsuarioConApellidoModificado()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);
        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        validadorTelefonoMock
            .Setup(v => v.EsValido("099123456"))
            .Returns(true);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Apellido = "Gomez" };

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
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);
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

        validadorTelefonoMock
            .Setup(v => v.EsValido("099123456"))
            .Returns(true);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest();

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual("Juan", resultado.Nombre);
        Assert.AreEqual("Perez", resultado.Apellido);
        Assert.AreEqual("099123456", resultado.Telefono);
        Assert.AreEqual("admin@gmail.com", resultado.Email);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_SoloTelefono_RetornaUsuarioConTelefonoModificado()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);
        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        validadorTelefonoMock
            .Setup(v => v.EsValido("098111222"))
            .Returns(true);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Telefono = "098111222" };

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual("098111222", resultado.Telefono);

        Assert.AreEqual("Juan", resultado.Nombre);
        Assert.AreEqual("Perez", resultado.Apellido);
        Assert.AreEqual("admin@gmail.com", resultado.Email);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_TelefonoInvalido_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        validadorTelefonoMock
            .Setup(v => v.EsValido("123"))
            .Returns(false);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Telefono = "123" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarUsuarioInterno(5, 99, request)
        );

        Assert.AreEqual("El teléfono no es válido", ex.Message);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_TelefonoNoValidoSegunValidador_NoModificaTelefono()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        validadorTelefonoMock
            .Setup(v => v.EsValido("123"))
            .Returns(false);

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Telefono = "123" };

        Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarUsuarioInterno(5, 99, request)
        );

        Assert.AreEqual("099123456", usuarioExistente.Telefono.Valor);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_ApellidoMayorAVeinticinco_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        validadorTelefonoMock
            .Setup(v => v.EsValido("099123456"))
            .Returns(true);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Apellido = "ApellidoDemasiadoDemasiadoLargo"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarUsuarioInterno(5, 99, request)
        );

        Assert.AreEqual("El apellido debe tener entre 3 y 25 caracteres", ex.Message);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_Telefono_UsuarioNoExiste_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        repoMock
            .Setup(r => r.FindById(5))
            .Returns((Usuario?)null);

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new UsuarioService(repoMock.Object, Mock.Of<IValidadorTelefono>());

        var request = new ModificarUsuarioRequest { Telefono = "098111222" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarUsuarioInterno(5, 99, request)
        );

        Assert.AreEqual("El usuario no existe", ex.Message);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_TelefonoNoCambia_NoValidaTelefono()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;
        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Telefono = "099123456" };

        service.ModificarUsuarioInterno(5, 99, request);

        validadorTelefonoMock.Verify(v => v.EsValido(It.IsAny<string>()), Times.Never);
    }

    // seccion de mail
    [TestMethod]
    public void ModificarUsuarioInterno_SoloEmail_RetornaUsuarioConEmailModificado()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);
        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        repoMock
            .Setup(r => r.ExisteEmail("nuevo@gmail.com"))
            .Returns(false);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Email = "nuevo@gmail.com" };

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual("nuevo@gmail.com", resultado.Email);
        Assert.AreEqual("Juan", resultado.Nombre);
        Assert.AreEqual("Perez", resultado.Apellido);
        Assert.AreEqual("099123456", resultado.Telefono);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_EmailFormatoInvalido_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        repoMock
            .Setup(r => r.ExisteEmail("correo-invalido"))
            .Returns(false);

        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Email = "correo-invalido" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarUsuarioInterno(5, 99, request)
        );

        Assert.AreEqual("El email debe tener un formato válido", ex.Message);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_EmailYaRegistrado_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        repoMock
            .Setup(r => r.ExisteEmail("repetido@gmail.com"))
            .Returns(true);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Email = "repetido@gmail.com" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarUsuarioInterno(5, 99, request)
        );

        Assert.AreEqual("El email ya está registrado", ex.Message);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_EmailYaRegistrado_NoModificaEmail()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Perez",
            "admin2@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        repoMock
            .Setup(r => r.ExisteEmail("repetido@gmail.com"))
            .Returns(true);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Email = "repetido@gmail.com" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarUsuarioInterno(5, 99, request)
        );

        Assert.AreEqual("El email ya está registrado", ex.Message);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_EmailNull_ConservaEmail()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);
        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Email = null };

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual("admin@gmail.com", resultado.Email);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_EmailVacio_ConservaEmail()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;
        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest { Email = string.Empty };

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual("admin@gmail.com", resultado.Email);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_EmailIgualAlActual_NoConsultaRepositorio()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Email = "admin@gmail.com"
        };

        service.ModificarUsuarioInterno(5, 99, request);

        repoMock.Verify(r => r.ExisteEmail(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_ApellidoMenorATres_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        validadorTelefonoMock
            .Setup(v => v.EsValido("099123456"))
            .Returns(true);

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Apellido = "Go"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarUsuarioInterno(5, 99, request)
        );

        Assert.AreEqual("El apellido debe tener entre 3 y 25 caracteres", ex.Message);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_UsuarioNoExiste_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var service = new UsuarioService(repoMock.Object, Mock.Of<IValidadorTelefono>());

        repoMock
            .Setup(r => r.FindById(5))
            .Returns((Usuario?)null);

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var request = new ModificarUsuarioRequest { Nombre = "Carlos" };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.ModificarUsuarioInterno(5, 99, request)
        );

        Assert.AreEqual("El usuario no existe", ex.Message);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_EmailIgualConDistintoCase_NoConsultaRepositorio()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);
        var usuarioLogueado = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin12@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Email = "ADMIN@GMAIL.COM"
        };

        service.ModificarUsuarioInterno(5, 99, request);

        repoMock.Verify(r => r.ExisteEmail(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void ModificarNombre_NombreConNumeros_LanzaExcepcionConMensajeCorrecto()
    {
        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            usuario.ModificarNombre("Ju4n")
        );

        Assert.AreEqual("El nombre no puede contener números", ex.Message);
    }
}
