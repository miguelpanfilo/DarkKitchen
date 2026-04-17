using Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacenamiento.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Property(u => u.Name)
            .IsRequired();

        builder.Property(u => u.LastName)
            .IsRequired();

        builder.Property(u => u.Role)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired();

            email.HasIndex(e => e.Value)
                .IsUnique();
        });

        builder.OwnsOne(u => u.Phone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("Phone")
                .IsRequired();
        });

        builder.OwnsOne(u => u.Password, password =>
        {
            password.Property(p => p.Hash)
                .HasColumnName("PasswordHash")
                .IsRequired();
        });
    }
}
