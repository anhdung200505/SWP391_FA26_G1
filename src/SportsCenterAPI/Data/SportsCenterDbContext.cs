using Microsoft.EntityFrameworkCore;
using SportsCenterAPI.Models;

namespace SportsCenterAPI.Data;

public class SportsCenterDbContext : DbContext
{
    public SportsCenterDbContext(DbContextOptions<SportsCenterDbContext> options)
        : base(options)
    {
    }

    public DbSet<Center> Centers { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =============================================
        // Center — dbo.Center
        // =============================================
        modelBuilder.Entity<Center>(entity =>
        {
            entity.ToTable("Center", "dbo");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.Property(e => e.Address).HasColumnName("address").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Hotline).HasColumnName("hotline").HasMaxLength(20);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(254);
        });

        // =============================================
        // Role — dbo.Role
        // =============================================
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role", "dbo");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(255);

            // NOTE: Column is "create_at" (typo in DB schema) — DO NOT rename the column
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
        });

        // =============================================
        // User — dbo.[User]
        // "User" is reserved in SQL Server — must use schema-qualified table name
        // =============================================
        modelBuilder.Entity<User>(entity =>
        {
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

            // Never select this in API projections
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
                  .HasDefaultValue("ACTIVE");

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasColumnType("datetime2")
                  .IsRequired()
                  .HasDefaultValueSql("SYSDATETIME()");

            // Foreign keys
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
        });
    }
}
