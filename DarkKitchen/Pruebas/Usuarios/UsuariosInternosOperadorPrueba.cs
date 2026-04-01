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
}
