using Almacenamiento.Configurations;
using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Almacenamiento;

public class SqlContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<Promotion> Promotions { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<PromotionProduct> PromotionProducts { get; set; } = null!;

    public SqlContext(DbContextOptions<SqlContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
        modelBuilder.ApplyConfiguration(new PromotionConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new PromotionProductConfiguration());
    }
}
