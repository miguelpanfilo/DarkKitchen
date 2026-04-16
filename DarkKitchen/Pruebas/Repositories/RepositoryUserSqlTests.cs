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
}
