using Almacenamiento;
using Dominio;

namespace Pruebas.InicioSesion;

[TestClass]
public class RepositorySessionTests
{
    [TestMethod]
    public void Add_ValidSession_ReturnsTheSameSession()
    {
        var repository = new RepositorySession();

        var expireDate = new DateTime(2026, 1, 1, 12, 0, 0);
        var session = new Session(5, expireDate);

        var result = repository.Add(session);

        Assert.IsNotNull(result);
        Assert.AreEqual(session, result);
        Assert.AreEqual(5, result.UserId);
        Assert.AreEqual(expireDate, result.ExpireDate);
        Assert.AreEqual(session.SessionId, result.SessionId);
    }

    [TestMethod]
    public void FindBySessionId_ExistingSession_ReturnsSession()
    {
        var repository = new RepositorySession();

        var session = new Session(1, new DateTime(2026, 1, 1, 12, 0, 0));

        repository.Add(session);

        var result = repository.FindBySessionId(session.SessionId);

        Assert.IsNotNull(result);
        Assert.AreEqual(session.SessionId, result.SessionId);
        Assert.AreEqual(1, result.UserId);
    }
}
