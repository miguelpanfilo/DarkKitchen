using Almacenamiento;
using Dominio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pruebas.Usuarios;

[TestClass]
public class RepositoryUserTests
{
    [TestMethod]
    public void FindByEmail_EmailExists_ReturnsUser()
    {
        var repo = new RepositoryUser();

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc"
        );

        repo.Add(user);

        var result = repo.FindByEmail("miguel@gmail.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("Miguel", result.Name);
    }

    [TestMethod]
    public void FindByEmail_EmailDoesNotExist_ReturnsNull()
    {
        var repo = new RepositoryUser();

        var result = repo.FindByEmail("noexiste@gmail.com");

        Assert.IsNull(result);
    }
}
