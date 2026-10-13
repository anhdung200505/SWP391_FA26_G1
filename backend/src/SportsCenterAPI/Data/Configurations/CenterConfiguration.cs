using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsCenterAPI.Models;

namespace SportsCenterAPI.Data.Configurations;

/// <summary>
/// Map class Center sang bảng dbo.Center.
/// </summary>
public class CenterConfiguration : IEntityTypeConfiguration<Center>
{
    public void Configure(EntityTypeBuilder<Center> entity)
    {
        entity.ToTable("Center", "dbo");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        entity.Property(e => e.Address).HasColumnName("address").HasMaxLength(255).IsRequired();
        entity.Property(e => e.Hotline).HasColumnName("hotline").HasMaxLength(20);
        entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(254);
    }
}
