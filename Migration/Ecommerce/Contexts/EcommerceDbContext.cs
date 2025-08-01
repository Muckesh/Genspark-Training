using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Contexts
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // user -> products
            // modelBuilder.Entity<Product>()
            //             .HasOne(p => p.User)
            //             .WithMany(u => u.Products)
            //             .HasForeignKey(p => p.UserId)
            //             .OnDelete(DeleteBehavior.Restrict);

            // user -> news
            modelBuilder.Entity<News>()
                        .HasOne(n => n.User)
                        .WithMany(u => u.News)
                        .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<Product>(entity =>
            {
                // user -> product
                entity.HasOne(p => p.User)
                    .WithMany(u => u.Products)
                    .HasForeignKey(p => p.UserId)
                    .HasConstraintName("FK_Product_User");

                // product -> categories
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .HasConstraintName("FK_Product_Category");

                // product -> colors
                entity.HasOne(p => p.Color)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.ColorId)
                    .HasConstraintName("FK_Product_Color");

                // product -> models
                entity.HasOne(p => p.Model)
                    .WithMany(m => m.Products)
                    .HasForeignKey(p => p.ModelId)
                    .HasConstraintName("FK_Product_Model");

            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasOne(od => od.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(od => od.OrderID)
                    .HasConstraintName("FK_OrderDetail_Order");

                entity.HasOne(od => od.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(od => od.ProductID)
                    .HasConstraintName("FK_OrderDetail_Product");

                entity.HasKey(od => new { od.OrderID, od.ProductID });
            });
            

        }
        
    }
}