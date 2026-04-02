using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class ConsultaUsuarioPruebas
{
    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreExistente_RetornaUsuariosCoincidentes()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuario1 = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario1.Id = 1;

        var usuario2 = Usuario.CrearInterno(
            "Juan",
            "Gomez",
            "juan2@gmail.com",
            "099111222",
            "PasswordSeguraaa4!",
            RolUsuario.Preparador
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario1, usuario2 });

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var resultado = service.BuscarUsuariosPorNombre("Juan");

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Nombre);
        Assert.AreEqual("Juan", resultado[1].Nombre);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreInexistente_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuario = Usuario.CrearInterno(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario });

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var resultado = service.BuscarUsuariosPorNombre("Juan");

        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_FiltraSoloLosUsuariosConEseNombre()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuario1 = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario1.Id = 1;

        var usuario2 = Usuario.CrearInterno(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            "099111222",
            "PasswordSeguraaa4!",
            RolUsuario.Preparador
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario1, usuario2 });

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var resultado = service.BuscarUsuariosPorNombre("Juan");

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Nombre);
        Assert.AreEqual("Perez", resultado[0].Apellido);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreConDistintoCase_RetornaCoincidencias()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario });

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var resultado = service.BuscarUsuariosPorNombre("juan");

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Nombre);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_SinUsuarios_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario>());

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var resultado = service.BuscarUsuariosPorNombre("Juan");

        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_MapeaCorrectamenteLosDatosDelUsuario()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 100;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario });

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var resultado = service.BuscarUsuariosPorNombre("Juan");

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(100, resultado[0].Id);
        Assert.AreEqual("Juan", resultado[0].Nombre);
        Assert.AreEqual("Perez", resultado[0].Apellido);
        Assert.AreEqual("juan@gmail.com", resultado[0].Email);
        Assert.AreEqual("Administrativo", resultado[0].Rol);
        Assert.AreEqual("099123456", resultado[0].Telefono);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_ApellidoExistente_RetornaUsuariosCoincidentes()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuario1 = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario1.Id = 1;

        var usuario2 = Usuario.CrearInterno(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            "099111222",
            "PasswordSeguraaa4!",
            RolUsuario.Preparador
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario1, usuario2 });

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var resultado = service.BuscarUsuariosPorApellido("Perez");

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Perez", resultado[0].Apellido);
        Assert.AreEqual("Perez", resultado[1].Apellido);
    }
}
