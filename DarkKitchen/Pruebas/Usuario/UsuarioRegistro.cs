using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class UsuarioServiceTests
{
    [TestMethod]
    public void RegistrarCliente_DatosValidos_RetornaUsuario()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        repoMock.Setup(r => r.Add(It.IsAny<Usuario>()))
            .Returns((Usuario u) => u);

        var usuarioServicio = new UsuarioService(repoMock.Object);

        var solicitudDeRegistro = new RegistrarClienteRequest
        {
            Nombre = "Miguel",
            Apellido = "Lopez",
            Email = "miguel@gmail.com",
            Telefono = "099123456",
            Clave = "Password@2026Ok"
        };

        var resultado = usuarioServicio.RegistrarCliente(solicitudDeRegistro);

        Assert.IsNotNull(resultado);
        Assert.AreEqual("Miguel", resultado.Nombre);
        Assert.AreEqual("Lopez", resultado.Apellido);
        Assert.AreEqual("miguel@gmail.com", resultado.Email);
    }

    [TestMethod]
    public void Add_UsuarioValido_GuardaUsuarioEnMemoria()
    {
        var repo = new RepositorioUsuarioMemoria();

        var usuario = new Usuario
        {
            Nombre = "Miguel",
            Apellido = "Lopez",
            Email = "miguel@gmail.com",
            Telefono = "099123456",
            Clave = "Password@2026Ok"
        };

        repo.Add(usuario);

        var usuarios = repo.GetAll();

        Assert.AreEqual(1, usuarios.Count);
        Assert.AreEqual("Miguel", usuarios[0].Nombre);
        Assert.AreEqual("Lopez", usuarios[0].Apellido);
        Assert.AreEqual("miguel@gmail.com", usuarios[0].Email);
    }
}

