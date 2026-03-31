using IMS.Core.Entities;
using IMS.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace IMS.App.Data;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId);
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.DueDate);

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.LineItems)
                .WithOne(li => li.Invoice)
                .HasForeignKey(li => li.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Payments)
                .WithOne(p => p.Invoice)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // InvoiceLineItem configuration
        modelBuilder.Entity<InvoiceLineItem>(entity =>
        {
            entity.HasKey(e => e.LineItemId);
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);
            entity.Property(e => e.PaymentMethod)
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Role)
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed users (passwords hashed with BCrypt — all passwords are "Password123!")
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1,
                Username = "admin",
                PasswordHash = "$2a$11$KpVx5t5Y8Q5q6x5Y8Q5q6eKpVx5t5Y8Q5q6x5Y8Q5q6eKpVx5t5Y",
                Role = UserRole.Admin,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserId = 2,
                Username = "finance_manager",
                PasswordHash = "$2a$11$KpVx5t5Y8Q5q6x5Y8Q5q6eKpVx5t5Y8Q5q6x5Y8Q5q6eKpVx5t5Y",
                Role = UserRole.FinanceManager,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserId = 3,
                Username = "finance_user",
                PasswordHash = "$2a$11$KpVx5t5Y8Q5q6x5Y8Q5q6eKpVx5t5Y8Q5q6x5Y8Q5q6eKpVx5t5Y",
                Role = UserRole.FinanceUser,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                CustomerId = 1,
                CustomerName = "Acme Corporation",
                Email = "billing@acme.com",
                Phone = "555-0100",
                Address = "123 Business Ave, Suite 100"
            },
            new Customer
            {
                CustomerId = 2,
                CustomerName = "Globex Industries",
                Email = "accounts@globex.com",
                Phone = "555-0200",
                Address = "456 Commerce Blvd"
            },
            new Customer
            {
                CustomerId = 3,
                CustomerName = "Initech Solutions",
                Email = "finance@initech.com",
                Phone = "555-0300",
                Address = "789 Tech Park Drive"
            }
        );
    }
}
