using System.Data.Common;
using System.Linq;
using Almacenamiento;
using Dominio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.Persistence;

namespace Pruebas.Repositories;

[TestClass]
public class RepositoryUserSqlTests
{
    private SqlContext _context = null!;
    private RepositoryUser _repository = null!;
    private DbConnection _connection = null!;

    [TestInitialize]
    public void Initialize()
    {
        _context = SqliteTestContextFactory.CreateContext(out _connection);
        _repository = new RepositoryUser(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [TestMethod]
    public void AddUser_WhenUserIsValid_PersistsUserInDatabase()
    {
        var phoneResult = Phone.TryMakePhone("099123456");
        Assert.IsTrue(phoneResult.Success);

        var userResult = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phoneResult.Value!,
            "PasswordSeguraee3!");

        Assert.IsTrue(userResult.Success);

        var user = userResult.Value!;
        user.Id = 1;

        _repository.Add(user);

        var savedUser = _context.Users.SingleOrDefault(u => u.Id == 1);

        Assert.IsNotNull(savedUser);
        Assert.AreEqual("Miguel", savedUser.Name);
        Assert.AreEqual("Lopez", savedUser.LastName);
        Assert.AreEqual("miguel@gmail.com", savedUser.Email.Value);
        Assert.AreEqual("099123456", savedUser.Phone.Value);
        Assert.AreEqual(UserRoles.Client, savedUser.Role);
        Assert.IsTrue(savedUser.VerifyPassword("PasswordSeguraee3!"));
    }

    [TestMethod]
    public void FindById_WhenUserExists_ReturnsUser()
    {
        var phoneResult = Phone.TryMakePhone("099123456");
        Assert.IsTrue(phoneResult.Success);

        var userResult = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phoneResult.Value!,
            "PasswordSeguraee3!");

        Assert.IsTrue(userResult.Success);

        var user = userResult.Value!;
        user.Id = 1;

        _context.Users.Add(user);
        _context.SaveChanges();

        var foundUser = _repository.FindById(1);

        Assert.IsNotNull(foundUser);
        Assert.AreEqual(1, foundUser.Id);
        Assert.AreEqual("Miguel", foundUser.Name);
        Assert.AreEqual("miguel@gmail.com", foundUser.Email.Value);
    }

    [TestMethod]
    public void FindById_WhenUserDoesNotExist_ReturnsNull()
    {
        var foundUser = _repository.FindById(999);

        Assert.IsNull(foundUser);
    }

    [TestMethod]
    public void FindByEmail_WhenEmailExists_ReturnsUser()
    {
        var phoneResult = Phone.TryMakePhone("099123456");
        Assert.IsTrue(phoneResult.Success);

        var userResult = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phoneResult.Value!,
            "PasswordSeguraee3!");

        Assert.IsTrue(userResult.Success);

        var user = userResult.Value!;
        user.Id = 1;

        _context.Users.Add(user);
        _context.SaveChanges();

        var foundUser = _repository.FindByEmail("miguel@gmail.com");

        Assert.IsNotNull(foundUser);
        Assert.AreEqual(1, foundUser.Id);
        Assert.AreEqual("Miguel", foundUser.Name);
        Assert.AreEqual("miguel@gmail.com", foundUser.Email.Value);
    }

    [TestMethod]
    public void FindByEmail_WhenEmailDoesNotExist_ReturnsNull()
    {
        var foundUser = _repository.FindByEmail("noexiste@gmail.com");

        Assert.IsNull(foundUser);
    }

    [TestMethod]
    public void EmailExist_WhenEmailExists_ReturnsTrue()
    {
        var phoneResult = Phone.TryMakePhone("099123456");
        Assert.IsTrue(phoneResult.Success);

        var userResult = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phoneResult.Value!,
            "PasswordSeguraee3!");

        Assert.IsTrue(userResult.Success);

        var user = userResult.Value!;
        user.Id = 1;

        _context.Users.Add(user);
        _context.SaveChanges();

        var exists = _repository.EmailExist("miguel@gmail.com");

        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void EmailExist_WhenEmailDoesNotExist_ReturnsFalse()
    {
        var exists = _repository.EmailExist("noexiste@gmail.com");

        Assert.IsFalse(exists);
    }

    [TestMethod]
    public void GetAll_WhenUsersExist_ReturnsAllUsers()
    {
        var phoneOneResult = Phone.TryMakePhone("099123456");
        var phoneTwoResult = Phone.TryMakePhone("091222333");

        Assert.IsTrue(phoneOneResult.Success);
        Assert.IsTrue(phoneTwoResult.Success);

        var firstUserResult = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phoneOneResult.Value!,
            "PasswordSeguraee3!");

        var secondUserResult = User.MakeClient(
            "Ana",
            "Perez",
            "ana@gmail.com",
            phoneTwoResult.Value!,
            "ClaveSeguraLargaa4!");

        Assert.IsTrue(firstUserResult.Success);
        Assert.IsTrue(secondUserResult.Success);

        var firstUser = firstUserResult.Value!;
        var secondUser = secondUserResult.Value!;

        firstUser.Id = 1;
        secondUser.Id = 2;

        _context.Users.Add(firstUser);
        _context.Users.Add(secondUser);
        _context.SaveChanges();

        var users = _repository.GetAll();

        Assert.AreEqual(2, users.Count);
        Assert.IsTrue(users.Any(u => u.Id == 1 && u.Email.Value == "miguel@gmail.com"));
        Assert.IsTrue(users.Any(u => u.Id == 2 && u.Email.Value == "ana@gmail.com"));
    }
}
