using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using E_Commerce_System.Models;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Description)
                      .HasMaxLength(1000);

                entity.Property(p => p.Price)
                      .IsRequired();

                entity.Property(p => p.Sizes)
                      .HasMaxLength(100);

                entity.Property(p => p.Colors)
                      .HasMaxLength(200);

                entity.Property(p => p.ImageUrl)
                      .HasMaxLength(500);

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.Description)
                      .HasMaxLength(300);

                entity.Property(c => c.IconClass)
                      .HasMaxLength(80);

                entity.Property(c => c.ImageUrl)
                      .HasMaxLength(500);
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.CustomerName)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(r => r.CustomerEmail)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(r => r.Comment)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.Property(r => r.Rating)
                      .IsRequired();

                entity.HasOne(r => r.Product)
                      .WithMany(p => p.Reviews)
                      .HasForeignKey(r => r.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.OrderNumber)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.HasIndex(o => o.OrderNumber)
                      .IsUnique();                   

                entity.Property(o => o.CustomerName)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(o => o.CustomerEmail)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(o => o.CustomerPhone)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(o => o.ShippingAddress)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(o => o.TotalAmount)
                      .IsRequired();

                entity.Property(o => o.Notes)
                      .HasMaxLength(500);

                entity.Property(o => o.Status)
                      .HasConversion<int>();
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);

                entity.Property(oi => oi.UnitPrice)
                      .IsRequired();

                entity.Property(oi => oi.SelectedSize)
                      .HasMaxLength(20);

                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Product)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(oi => oi.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.Name)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(o => o.Role)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(o => o.Bio)
                      .HasMaxLength(1000);

                entity.Property(o => o.ImageUrl)
                      .HasMaxLength(500);

                entity.Property(o => o.Facebook)
                      .HasMaxLength(200);

                entity.Property(o => o.Instagram)
                      .HasMaxLength(200);

                entity.Property(o => o.LinkedIn)
                      .HasMaxLength(200);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(s => s.ContactPerson)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(s => s.Email)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(s => s.Phone)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(s => s.Address)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(s => s.ProductTypes)
                      .HasMaxLength(300);

                entity.Property(s => s.Notes)
                      .HasMaxLength(500);
            });
        }
    }
