using Almacenamiento;
using Dominio;

namespace Pruebas;

[TestClass]
public class RepositoryUserTests
{
    private static Phone CreateValidPhone(string number)
    {
        var phoneResult = Phone.TryMakePhone(number);
        Assert.IsTrue(phoneResult.Success);
        Assert.IsNotNull(phoneResult.Value);
        return phoneResult.Value!;
    }

    private static User CreateValidUser(
        string name,
        string lastName,
        string email,
        string phone,
        string password)
    {
        var validPhone = CreateValidPhone(phone);

        var userResult = User.MakeClient(
            name,
            lastName,
            email,
            validPhone,
            password);

        Assert.IsTrue(userResult.Success);
        Assert.IsNotNull(userResult.Value);
        return userResult.Value!;
    }

    [TestMethod]
    public void FindByEmail_EmailExists_ReturnsUser()
    {
        var repo = new RepositoryUser();

        var user = CreateValidUser(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
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
    public void FindByEmail_InvalidEmail_ReturnsNull()
    {
        var repo = new RepositoryUser();

        var result = repo.FindByEmail("correo-invalido");

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

        var user1 = CreateValidUser(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2Ok6Abc");

        var user2 = CreateValidUser(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099999999",
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

        var user = CreateValidUser(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
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
    public void EmailExist_InvalidEmail_ReturnsFalse()
    {
        var repo = new RepositoryUser();

        var result = repo.EmailExist("correo-invalido");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void FindById_ExistingId_ReturnsUser()
    {
        var repo = new RepositoryUser();

        var user = CreateValidUser(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
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

        var user = CreateValidUser(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2Ok6Abc");

        var addedUser = repo.Add(user);

        repo.Remove(addedUser.Id);

        var result = repo.FindById(addedUser.Id);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Add_NewUser_AssignsIncrementalId()
    {
        var repo = new RepositoryUser();

        var user1 = CreateValidUser(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2Ok6Abc");

        var user2 = CreateValidUser(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099999999",
            "Password@2Ok6Abc");

        var userAddedFirst = repo.Add(user1);
        var userAddedSecond = repo.Add(user2);

        var firstAddInTheRow = 1;
        var secondAddInTheRow = 2;

        Assert.AreEqual(firstAddInTheRow, userAddedFirst.Id);
        Assert.AreEqual(secondAddInTheRow, userAddedSecond.Id);
    }

    [TestMethod]
    public void Add_User_AssignsIdGreaterThanZero()
    {
        var repo = new RepositoryUser();

        var user = CreateValidUser(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2Ok6Abc");

        var userNuevo = repo.Add(user);

        Assert.IsTrue(userNuevo.Id > 0);
    }

    [TestMethod]
    public void Add_User_SavedCorrectly()
    {
        var repo = new RepositoryUser();

        var user = CreateValidUser(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            "099123456",
            "Password@2Ok6Abc");

        repo.Add(user);

        var users = repo.GetAll();

        Assert.AreEqual(1, users.Count);
        Assert.AreEqual("Miguel", users[0].Name);
        Assert.AreEqual("Lopez", users[0].LastName);
    }
}
