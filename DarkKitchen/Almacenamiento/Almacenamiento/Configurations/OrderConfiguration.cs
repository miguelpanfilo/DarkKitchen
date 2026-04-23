using Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacenamiento.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedOnAdd();

        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(o => o.OrderNumber).IsUnique();

        builder.Property(o => o.ClientId).IsRequired();
        builder.HasOne(o => o.Client)
               .WithMany()
               .HasForeignKey(o => o.ClientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(o => o.DeliveryType).HasConversion<string>().IsRequired();
        builder.Property(o => o.Status).HasConversion<string>().IsRequired();

        builder.OwnsOne(o => o.Address, addr =>
        {
            addr.Property(a => a.Street).HasColumnName("Street").IsRequired();
            addr.Property(a => a.DoorNumber).HasColumnName("DoorNumber").IsRequired();
            addr.Property(a => a.Apartment).HasColumnName("Apartment");
        });

        builder.Property(o => o.Subtotal).HasPrecision(18, 2).IsRequired();
        builder.Property(o => o.ShippingCost).HasPrecision(18, 2).IsRequired();
        builder.Property(o => o.Total).HasPrecision(18, 2).IsRequired();

        builder.HasMany(o => o.Items)
               .WithOne()
               .HasForeignKey(i => i.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
