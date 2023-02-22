using Microsoft.EntityFrameworkCore;

namespace ProductService.Repositories.Entities; 

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<ProductEntity> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductEntity>()
                    .HasIndex(p => p.Id);

        base.OnModelCreating(modelBuilder);
    }
}
