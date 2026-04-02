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
}
