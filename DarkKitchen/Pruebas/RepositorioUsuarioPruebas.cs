using Almacenamiento;
using Dominio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pruebas.Usuarios;

[TestClass]
public class RepositorioUsuarioTests
{
    [TestMethod]
    public void ObtenerPorEmail_EmailExiste_RetornaUsuario()
    {
        var repo = new RepositorioUsuario();

        var usuario = Usuario.CrearCliente(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2Ok6Abc"
        );

        repo.Add(usuario);

        var resultado = repo.ObtenerPorEmail("miguel@gmail.com");

        Assert.IsNotNull(resultado);
        Assert.AreEqual("Miguel", resultado.Nombre);
    }

    [TestMethod]
    public void ObtenerPorEmail_EmailNoExiste_RetornaNull()
    {
        var repo = new RepositorioUsuario();

        var resultado = repo.ObtenerPorEmail("noexiste@gmail.com");

        Assert.IsNull(resultado);
    }
}
