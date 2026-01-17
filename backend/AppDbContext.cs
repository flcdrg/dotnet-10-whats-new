using Demo.Api.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Pet store database context for Entity Framework Core.
/// Configured for SQLite in development and SQL Server in production.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Accessory> Accessories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Accessory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.Description).HasMaxLength(1000);
        });

        // Seed sample accessories
        modelBuilder.Entity<Accessory>().HasData(
            new Accessory { Id = 1, Name = "Leather Collar", Category = "Collars", Price = 24.99m, StockQuantity = 50, Description = "Durable leather collar with brass buckle" },
            new Accessory { Id = 2, Name = "Interactive Toy Ball", Category = "Toys", Price = 12.99m, StockQuantity = 100, Description = "Bouncy ball with bell inside for interactive play" },
            new Accessory { Id = 3, Name = "Soft Dog Bed", Category = "Bedding", Price = 49.99m, StockQuantity = 30, Description = "Comfortable orthopedic dog bed with memory foam" },
            new Accessory { Id = 4, Name = "Rope Toy", Category = "Toys", Price = 9.99m, StockQuantity = 75, Description = "Braided rope toy for tug of war" },
            new Accessory { Id = 5, Name = "Grooming Brush", Category = "Grooming", Price = 14.99m, StockQuantity = 60, Description = "Slicker brush for removing loose fur and mats" },
            new Accessory { Id = 6, Name = "Pet Food Bowl Set", Category = "Feeding", Price = 19.99m, StockQuantity = 80, Description = "Stainless steel food and water bowls with rubber base" },
            new Accessory { Id = 7, Name = "Retractable Leash", Category = "Leashes", Price = 22.99m, StockQuantity = 45, Description = "16ft retractable leash with locking mechanism" },
            new Accessory { Id = 8, Name = "Pet Crate", Category = "Housing", Price = 79.99m, StockQuantity = 20, Description = "Durable plastic crate for training and travel" },
            new Accessory { Id = 9, Name = "Chew Stick Pack", Category = "Treats", Price = 8.99m, StockQuantity = 120, Description = "Natural rawhide chew sticks (pack of 5)" },
            new Accessory { Id = 10, Name = "Pet Carrier Backpack", Category = "Travel", Price = 39.99m, StockQuantity = 25, Description = "Ventilated backpack carrier for pets up to 15 lbs" }
        );
    }
}
