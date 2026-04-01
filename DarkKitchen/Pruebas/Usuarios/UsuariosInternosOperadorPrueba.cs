using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
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
}
