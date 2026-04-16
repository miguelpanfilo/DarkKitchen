using Almacenamiento.Configurations;
using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Almacenamiento;

public class SqlContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; } = null!;

    public SqlContext(DbContextOptions<SqlContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
    }
}
