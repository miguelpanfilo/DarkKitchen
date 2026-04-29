using System.Data.Common;
using Almacenamiento;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Pruebas.Persistence;

public static class SqliteTestContextFactory
{
    public static SqlContext CreateContext(out DbConnection connection)
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<SqlContext>()
            .UseSqlite(connection)
            .Options;

        var context = new SqlContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}
