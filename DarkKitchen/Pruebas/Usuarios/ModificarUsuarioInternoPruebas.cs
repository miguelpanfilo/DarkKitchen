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

        validadorTelefonoMock
            .Setup(v => v.EsValido("099123456"))
            .Returns(true);

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
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        validadorTelefonoMock
            .Setup(v => v.EsValido("098111222"))
            .Returns(true);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Telefono = "098111222"
        };

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual("098111222", resultado.Telefono);

        Assert.AreEqual("Juan", resultado.Nombre);
        Assert.AreEqual("Perez", resultado.Apellido);
        Assert.AreEqual("admin@gmail.com", resultado.Email);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void ModificarUsuarioInterno_TelefonoInvalido_LanzaExcepcion()
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

        validadorTelefonoMock
            .Setup(v => v.EsValido("123"))
            .Returns(false);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Telefono = "123"
        };

        service.ModificarUsuarioInterno(5, 99, request);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_TelefonoInvalido_NoModificaTelefono()
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

        validadorTelefonoMock
            .Setup(v => v.EsValido("123"))
            .Returns(false);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Telefono = "123"
        };

        try
        {
            service.ModificarUsuarioInterno(5, 99, request);
        }
        catch (UsuarioExcepcion)
        {
        }

        Assert.AreEqual("099123456", usuarioExistente.Telefono);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void ModificarUsuarioInterno_Telefono_UsuarioNoExiste_LanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        repoMock
            .Setup(r => r.FindById(5))
            .Returns((Usuario?)null);

        var service = new UsuarioService(repoMock.Object, Mock.Of<IValidadorTelefono>());

        var request = new ModificarUsuarioRequest
        {
            Telefono = "098111222"
        };

        service.ModificarUsuarioInterno(5, 99, request);
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
            Telefono = "099123456"
        };

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
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        repoMock
            .Setup(r => r.ExisteEmail("nuevo@gmail.com"))
            .Returns(false);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Email = "nuevo@gmail.com"
        };

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
            "099123456",
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

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Email = "correo-invalido"
        };

        try
        {
            service.ModificarUsuarioInterno(5, 99, request);
            Assert.Fail("Se esperaba excepción");
        }
        catch (UsuarioExcepcion ex)
        {
            Assert.AreEqual("El email debe tener un formato válido", ex.Message);
        }
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
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        repoMock
            .Setup(r => r.ExisteEmail("repetido@gmail.com"))
            .Returns(true);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Email = "repetido@gmail.com"
        };

        try
        {
            service.ModificarUsuarioInterno(5, 99, request);
            Assert.Fail("Se esperaba excepción");
        }
        catch (UsuarioExcepcion ex)
        {
            Assert.AreEqual("El email ya está registrado", ex.Message);
        }
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
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        repoMock
            .Setup(r => r.ExisteEmail("repetido@gmail.com"))
            .Returns(true);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var request = new ModificarUsuarioRequest
        {
            Email = "repetido@gmail.com"
        };

        try
        {
            service.ModificarUsuarioInterno(5, 99, request);
        }
        catch (UsuarioExcepcion)
        {
        }

        Assert.AreEqual("admin@gmail.com", usuarioExistente.Email.ToString());
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
            Email = null
        };

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
            Email = string.Empty
        };

        var resultado = service.ModificarUsuarioInterno(5, 99, request);

        Assert.AreEqual("admin@gmail.com", resultado.Email);
    }
}
