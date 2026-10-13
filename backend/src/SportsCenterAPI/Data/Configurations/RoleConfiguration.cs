using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsCenterAPI.Models;

namespace SportsCenterAPI.Data.Configurations;

/// <summary>
/// Map class Role sang bảng dbo.Role.
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> entity)
    {
        entity.ToTable("Role", "dbo");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        entity.HasIndex(e => e.Name).IsUnique();
        entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(255);

        // Cột DB đặt tên "create_at" (thiếu chữ d). Giữ nguyên, không đổi tên cột.
        entity.Property(e => e.CreateAt)
              .HasColumnName("create_at")
              .HasColumnType("datetime2")
              .IsRequired()
              .HasDefaultValueSql("SYSDATETIME()");

        entity.Property(e => e.UpdatedAt)
              .HasColumnName("updated_at")
              .HasColumnType("datetime2")
              .IsRequired()
              .HasDefaultValueSql("SYSDATETIME()");
    }
}
