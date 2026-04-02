using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class EliminacionUsuarioPruebas
{
    [TestMethod]
    public void EliminarUsuarioInterno_DatosValidos_LlamaRemoveUnaVez()
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

        service.EliminarUsuarioInterno(5, 99);

        repoMock.Verify(r => r.Remove(5), Times.Once);
    }
}
