using GamerCore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GamerCore.Infrastructure
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var now = DateTime.UtcNow;
            var productsToUpdateDict = new Dictionary<int, Product>();

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(p => p.CreatedAt).IsModified = false;
                    entry.Entity.UpdatedAt = now;
                }

                // Skip top-level entities
                if (entry.Entity is Product) continue;
                if (entry.Entity is Category) continue;

                // Save affected products
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    if (entry.Entity is ProductCategory productCategory)
                    {
                        productsToUpdateDict[productCategory.ProductId] = productCategory.Product;
                    }
                    else if (entry.Entity is ProductDetail productDetail)
                    {
                        productsToUpdateDict[productDetail.ProductId] = productDetail.Product;
                    }
                    else if (entry.Entity is ProductImage productImage)
                    {
                        productsToUpdateDict[productImage.ProductId] = productImage.Product;
                    }
                    else if (entry.Entity is ProductReview productReview)
                    {
                        productsToUpdateDict[productReview.ProductId] = productReview.Product;
                    }
                }
            }

            if (productsToUpdateDict.Count > 0)
            {
                foreach (var kvp in productsToUpdateDict)
                {
                    var product = kvp.Value;
                    var entry = Entry(product);

                    if (entry.State == EntityState.Detached)
                    {
                        Attach(product);
                        entry = Entry(product);
                    }

                    if (entry.State == EntityState.Unchanged || entry.State == EntityState.Modified)
                    {
                        product.UpdatedAt = now;
                        entry.State = EntityState.Modified;
                        entry.Property(p => p.CreatedAt).IsModified = false;
                    }
                }
            }
        }
    }
}