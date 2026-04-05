using Almacenamiento;
using Dominio;

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
            "Password@2Ok6Abc");

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

    [TestMethod]
    public void GetAll_WithoutUsers_ReturnsEmptyList()
    {
        var repo = new RepositoryUser();

        var result = repo.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetAll_WithUsers_ReturnsAllUsers()
    {
        var repo = new RepositoryUser();

        var user1 = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc");

        var user2 = User.MakeClient(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099999999"),
            "Password@2Ok6Abc");

        repo.Add(user1);
        repo.Add(user2);

        var result = repo.GetAll();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Miguel", result[0].Name);
        Assert.AreEqual("Juan", result[1].Name);
    }

    [TestMethod]
    public void EmailExist_EmailExists_ReturnsTrue()
    {
        var repo = new RepositoryUser();

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc");

        repo.Add(user);

        var result = repo.EmailExist("miguel@gmail.com");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void EmailExist_EmailDoesNotExist_ReturnsFalse()
    {
        var repo = new RepositoryUser();

        var result = repo.EmailExist("noexiste@gmail.com");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void FindById_ExistingId_ReturnsUser()
    {
        var repo = new RepositoryUser();

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc");

        var addedUser = repo.Add(user);

        var result = repo.FindById(addedUser.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(addedUser.Id, result.Id);
    }

    [TestMethod]
    public void FindById_NonExistingId_ReturnsNull()
    {
        var repo = new RepositoryUser();

        var result = repo.FindById(999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Remove_ExistingId_RemovesUser()
    {
        var repo = new RepositoryUser();

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc");

        var addedUser = repo.Add(user);

        repo.Remove(addedUser.Id);

        var result = repo.FindById(addedUser.Id);

        Assert.IsNull(result);
    }
}
