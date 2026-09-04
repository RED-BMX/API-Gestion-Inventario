using API_Gestion_Inventario.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Gestion_Inventario.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<InventoryMovement> InventoryMovements { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Product
        modelBuilder.Entity<Product>()
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);
        
        modelBuilder.Entity<Category>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        modelBuilder.Entity<Category>()
            .Property(c => c.Description)
            .HasMaxLength(300);
        
        modelBuilder.Entity<Product>()
            .Property(p => p.Description)
            .HasMaxLength(500);
        
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(10, 2);
        
        modelBuilder.Entity<Product>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Product_Price",
                "\"Price\" >= 0"));
        
        modelBuilder.Entity<Product>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Product_Stock",
                "\"Stock\" >= 0"));
        
        modelBuilder.Entity<Product>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Product_MinimumStock",
                "\"MinimumStock\" >= 0"));
        
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.CategoryId);

        // Category -> Products
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Product -> InventoryMovements
        modelBuilder.Entity<InventoryMovement>()
            .HasOne(m => m.Product)
            .WithMany(p => p.InventoryMovements)
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<InventoryMovement>()
            .Property(m => m.Type)
            .HasConversion<string>();

        // Role -> Users
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Configuración de Role
        modelBuilder.Entity<Role>()
            .Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();
        
        // Configuración de nombres únicos
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        // Configuración de InventoryMovement
        modelBuilder.Entity<InventoryMovement>()
            .Property(m => m.Description)
            .HasMaxLength(500);
        
        modelBuilder.Entity<InventoryMovement>()
            .Property(m => m.Date)
            .IsRequired();
        
        modelBuilder.Entity<InventoryMovement>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_InventoryMovement_Quantity",
                "\"Quantity\" > 0"));
    }
}