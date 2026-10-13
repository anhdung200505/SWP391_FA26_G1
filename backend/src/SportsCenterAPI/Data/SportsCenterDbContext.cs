using Microsoft.EntityFrameworkCore;
using SportsCenterAPI.Models;

namespace SportsCenterAPI.Data;

/// <summary>
/// Kết nối database. Cấu hình từng bảng nằm ở Data/Configurations.
/// </summary>
public class SportsCenterDbContext : DbContext
{
    public SportsCenterDbContext(DbContextOptions<SportsCenterDbContext> options)
        : base(options)
    {
    }

    // Thứ tự theo phụ thuộc: Center và Role có trước, User trỏ tới cả hai.
    public DbSet<Center> Centers { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Nạp mọi class IEntityTypeConfiguration trong project.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SportsCenterDbContext).Assembly);
    }
}
