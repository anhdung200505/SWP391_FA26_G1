using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsCenterAPI.Constants;
using SportsCenterAPI.Models;

namespace SportsCenterAPI.Data.Configurations;

/// <summary>
/// Map class User sang bảng dbo.[User].
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        // "User" là từ khóa của SQL Server nên phải ghi kèm schema dbo.
        entity.ToTable("User", "dbo");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        entity.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();
        entity.Property(e => e.CenterId).HasColumnName("center_id").IsRequired();

        entity.Property(e => e.Username)
              .HasColumnName("username")
              .HasMaxLength(50)
              .IsRequired();
        entity.HasIndex(e => e.Username).IsUnique();

        // Hash mật khẩu, không được đưa vào response API.
        entity.Property(e => e.PasswordHash)
              .HasColumnName("password_hash")
              .HasMaxLength(255)
              .IsRequired();

        entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
        entity.Property(e => e.Qualification).HasColumnName("qualification").HasMaxLength(255);

        entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(254).IsRequired();
        entity.HasIndex(e => e.Email).IsUnique();

        entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(20);
        entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth").HasColumnType("date");
        entity.Property(e => e.Gender).HasColumnName("gender").HasMaxLength(20);

        entity.Property(e => e.Status)
              .HasColumnName("status")
              .HasMaxLength(20)
              .IsRequired()
              .HasDefaultValue(UserStatuses.Active);

        entity.Property(e => e.CreatedAt)
              .HasColumnName("created_at")
              .HasColumnType("datetime2")
              .IsRequired()
              .HasDefaultValueSql("SYSDATETIME()");

        // Không cho xóa Role hoặc Center khi còn user đang trỏ tới.
        entity.HasOne(e => e.Role)
              .WithMany(r => r.Users)
              .HasForeignKey(e => e.RoleId)
              .OnDelete(DeleteBehavior.Restrict)
              .HasConstraintName("FK_User_Role");

        entity.HasOne(e => e.Center)
              .WithMany(c => c.Users)
              .HasForeignKey(e => e.CenterId)
              .OnDelete(DeleteBehavior.Restrict)
              .HasConstraintName("FK_User_Center");
    }
}
