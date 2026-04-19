using Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacenamiento.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(500);
        builder.Property(p => p.CommercialLine).IsRequired();
        builder.Property(p => p.Category).IsRequired();
        builder.Property(p => p.Price).IsRequired();
        builder.Property(p => p.IsActive).IsRequired();
        builder.Property(p => p.IsDeleted).IsRequired();

        builder.Ignore(p => p.Photos);
    }
}
