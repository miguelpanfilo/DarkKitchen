using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class AuditLogConfiguration :
    IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(log => log.Id);
        builder.Property(log => log.DateTime).IsRequired();
        builder.Property(log =>
            log.EntityName).IsRequired().HasMaxLength(50);
        builder.Property(log => log.EntityId).IsRequired();
        builder.Property(log =>
            log.ChangeDescription).IsRequired().HasMaxLength(500);
        builder.Property(log =>
            log.ResponsibleUser).IsRequired().HasMaxLength(120);
    }
}
