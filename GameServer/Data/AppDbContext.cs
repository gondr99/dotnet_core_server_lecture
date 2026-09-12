using GameServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    //엔티티로 만들어진 테이블을 선언한다. Linq 쿼리가 이걸로 시작한다.
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}