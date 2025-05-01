using System.Diagnostics;
using GamerCore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GamerCore.Infrastructure
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options)
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

            // Configure composite key
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            // Configure default values for timestamps
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType)))
            {
                var entity = modelBuilder.Entity(entityType.ClrType);

                // CreatedAt is handled explicitly but can still use ValueGenerateOnAdd()
                // for extra clarity
                entity.Property(nameof(BaseEntity.CreatedAt))
                    .HasDefaultValueSql("GETUTCDATE()")
                    .ValueGeneratedOnAdd();

                // Do not use ValueGeneratedOnAddOrUpdate(), already handle this explicitly
                entity.Property(nameof(BaseEntity.UpdatedAt))
                    .HasDefaultValueSql("GETUTCDATE()");
            }
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
                    // Track as unchanged
                    var product = kvp.Value;
                    Attach(product);

                    var entry = Entry(product);

                    Debug.Assert(entry.State == EntityState.Unchanged);

                    product.UpdatedAt = now;
                    entry.State = EntityState.Modified;

                    Debug.Assert(entry.State == EntityState.Modified);

                    entry.Property(p => p.CreatedAt).IsModified = false;
                }
            }
        }
    }
}