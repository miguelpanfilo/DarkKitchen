using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class ShippingTypeConfiguration : IEntityTypeConfiguration<ShippingType>
{
    public void Configure(EntityTypeBuilder<ShippingType> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired();
        builder.Property(s => s.Cost).IsRequired().HasPrecision(18, 2);
    }
}
