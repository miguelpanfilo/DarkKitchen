using Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacenamiento.Configurations;

public class PromotionProductConfiguration : IEntityTypeConfiguration<PromotionProduct>
{
    public void Configure(EntityTypeBuilder<PromotionProduct> builder)
    {
        builder.ToTable("PromotionProducts");

        builder.HasKey(pp => new { pp.PromotionId, pp.ProductId });

        builder.Property(pp => pp.PromotionId)
            .IsRequired();

        builder.Property(pp => pp.ProductId)
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
