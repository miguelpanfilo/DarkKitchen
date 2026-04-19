using System;
using System.Data.Common;
using System.Linq;
using Almacenamiento;
using Almacenamiento.Repositories;
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

    [TestMethod]
    public void FindBySessionId_WhenSessionDoesNotExist_ReturnsNull()
    {
        var foundSession = _repository.FindBySessionId(Guid.NewGuid());

        Assert.IsNull(foundSession);
    }

    [TestMethod]
    public void AddSession_WhenTwoSessionsAreAdded_PersistsBothSessions()
    {
        var firstSession = new Session(1, new DateTime(2026, 4, 16, 12, 0, 0));
        var secondSession = new Session(2, new DateTime(2026, 4, 16, 13, 0, 0));

        _repository.Add(firstSession);
        _repository.Add(secondSession);

        var sessions = _context.Sessions.ToList();

        Assert.AreEqual(2, sessions.Count);
    }

    [TestMethod]
    public void RemoveAll_WhenSessionsExist_RemovesAllSessions()
    {
        var firstSession = new Session(1, new DateTime(2026, 4, 16, 12, 0, 0));
        var secondSession = new Session(2, new DateTime(2026, 4, 16, 13, 0, 0));

        _context.Sessions.Add(firstSession);
        _context.Sessions.Add(secondSession);
        _context.SaveChanges();

        _repository.RemoveAll();

        var sessions = _context.Sessions.ToList();

        Assert.AreEqual(0, sessions.Count);
    }

    [TestMethod]
    public void RemoveAll_WhenThereAreNoSessions_DoesNothing()
    {
        _repository.RemoveAll();

        var sessions = _context.Sessions.ToList();

        Assert.AreEqual(0, sessions.Count);
    }

    [TestMethod]
    public void FindBySessionId_WhenThereAreMultipleSessions_ReturnsMatchingSession()
    {
        var firstSession = new Session(1, new DateTime(2026, 4, 16, 12, 0, 0));
        var secondSession = new Session(2, new DateTime(2026, 4, 16, 13, 0, 0));

        _context.Sessions.Add(firstSession);
        _context.Sessions.Add(secondSession);
        _context.SaveChanges();

        var foundSession = _repository.FindBySessionId(secondSession.SessionId);

        Assert.IsNotNull(foundSession);
        Assert.AreEqual(secondSession.SessionId, foundSession.SessionId);
        Assert.AreEqual(2, foundSession.UserId);
        Assert.AreEqual(new DateTime(2026, 4, 16, 13, 0, 0), foundSession.ExpireDate);
    }

    [TestMethod]
    public void RemoveAll_ThenAddNewSession_OnlyLastSessionRemainsInDatabase()
    {
        var firstSession = new Session(1, new DateTime(2026, 4, 16, 12, 0, 0));
        var secondSession = new Session(2, new DateTime(2026, 4, 16, 13, 0, 0));

        _repository.Add(firstSession);

        _repository.RemoveAll();

        _repository.Add(secondSession);

        var sessions = _context.Sessions.ToList();

        Assert.AreEqual(1, sessions.Count);
        Assert.AreEqual(secondSession.SessionId, sessions[0].SessionId);
        Assert.AreEqual(2, sessions[0].UserId);
        Assert.AreEqual(new DateTime(2026, 4, 16, 13, 0, 0), sessions[0].ExpireDate);
    }
}
