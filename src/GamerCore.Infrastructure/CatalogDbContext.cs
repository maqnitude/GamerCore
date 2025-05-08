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
            var productIdsToUpdate = new HashSet<int>();

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
                        productIdsToUpdate.Add(productCategory.ProductId);
                    }
                    else if (entry.Entity is ProductDetail productDetail)
                    {
                        productIdsToUpdate.Add(productDetail.ProductId);
                    }
                    else if (entry.Entity is ProductImage productImage)
                    {
                        productIdsToUpdate.Add(productImage.ProductId);
                    }
                    else if (entry.Entity is ProductReview productReview)
                    {
                        productIdsToUpdate.Add(productReview.ProductId);
                    }
                }
            }

            if (productIdsToUpdate.Count > 0)
            {
                var productsToUpdate = Products.Where(p => productIdsToUpdate.Contains(p.ProductId));
                foreach (var product in productsToUpdate)
                {
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