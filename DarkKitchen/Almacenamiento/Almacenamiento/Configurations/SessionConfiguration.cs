using Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacenamiento.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");

        builder.HasKey(s => s.SessionId);

        builder.Property(s => s.SessionId)
            .ValueGeneratedNever();

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.ExpireDate)
            .IsRequired();
    }
}
