using System;
using System.Data.Common;
using System.Linq;
using Almacenamiento;
using Dominio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.Persistence;

namespace Pruebas.Repositories;

[TestClass]
public class RepositorySessionSqlTests
{
    private SqlContext _context = null!;
    private RepositorySession _repository = null!;
    private DbConnection _connection = null!;

    [TestInitialize]
    public void Initialize()
    {
        _context = SqliteTestContextFactory.CreateContext(out _connection);
        _repository = new RepositorySession(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
        _connection?.Dispose();
    }

    [TestMethod]
    public void AddSession_WhenSessionIsValid_PersistsSessionInDatabase()
    {
        var expireDate = new DateTime(2026, 4, 16, 12, 0, 0);
        var session = new Session(5, expireDate);

        _repository.Add(session);

        var savedSession = _context.Sessions.SingleOrDefault(s => s.SessionId == session.SessionId);

        Assert.IsNotNull(savedSession);
        Assert.AreEqual(5, savedSession.UserId);
        Assert.AreEqual(expireDate, savedSession.ExpireDate);
    }

    [TestMethod]
    public void FindBySessionId_WhenSessionExists_ReturnsSession()
    {
        var expireDate = new DateTime(2026, 4, 16, 12, 0, 0);
        var session = new Session(5, expireDate);

        _context.Sessions.Add(session);
        _context.SaveChanges();

        var foundSession = _repository.FindBySessionId(session.SessionId);

        Assert.IsNotNull(foundSession);
        Assert.AreEqual(session.SessionId, foundSession.SessionId);
        Assert.AreEqual(5, foundSession.UserId);
        Assert.AreEqual(expireDate, foundSession.ExpireDate);
    }
}
