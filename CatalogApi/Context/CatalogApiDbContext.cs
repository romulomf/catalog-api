using CatalogApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Context;

public class CatalogApiDbContext(DbContextOptions<CatalogApiDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
	public DbSet<Category> Categories { get; set; }

	public DbSet<Product> Products { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		//modelBuilder.Entity<Category>().ToTable("CATEGORIES");
		//modelBuilder.Entity<Category>().HasKey(c => c.Id);
		//modelBuilder.Entity<Category>().Property(c => c.Id).HasColumnName("ID").HasColumnType("INT");
		//modelBuilder.Entity<Category>().Property(c => c.Name).HasColumnName("NAME").HasColumnType("NVARCHAR(MAX)").IsRequired();
		//modelBuilder.Entity<Category>().Property(c => c.ImageUrl).HasColumnName("IMAGE_URL").HasColumnType("NVARCHAR(MAX)");
		//modelBuilder.Entity<Category>().HasMany<Product>(c => c.Products).WithOne(p => p.Category);
		// Índices
		//modelBuilder.Entity<Category>().HasIndex(c => c.Id).HasDatabaseName("PK_CATEGORY_ID").IsUnique(true);

		//modelBuilder.Entity<Product>().ToTable("PRODUCTS");
		//modelBuilder.Entity<Product>().HasKey(p => p.Id);
		//modelBuilder.Entity<Product>().Property(p => p.Id).HasColumnName("ID").HasColumnType("INT");
		//modelBuilder.Entity<Product>().Property(p => p.Name).HasColumnName("NAME").HasColumnType("NVARCHAR(MAX)").IsRequired();
		//modelBuilder.Entity<Product>().Property(p => p.Description).HasColumnName("DESCRIPTION").HasColumnType("NVARCHAR(MAX)");
		//modelBuilder.Entity<Product>().Property(p => p.ImageUrl).HasColumnName("IMAGE_URL").HasColumnType("NVARCHAR(MAX)");
		//modelBuilder.Entity<Product>().Property(p => p.Stock).HasColumnName("STOCK").HasColumnType("SMALLMONEY").IsRequired();
		//modelBuilder.Entity<Product>().Property(p => p.RegisterDate).HasColumnName("REGISTER_DATE").HasColumnType("SMALLDATETIME").IsRequired();
		//modelBuilder.Entity<Product>().HasIndex(p => p.Id).HasDatabaseName("FK_PRODUCT_ID").IsUnique();
		modelBuilder.Entity<Product>().HasOne<Category>(p => p.Category).WithMany(c => c.Products).HasForeignKey("CATEGORY_ID").HasConstraintName("FK_CATEGORY_ID");
		// Índices
		modelBuilder.Entity<Product>().HasIndex("CATEGORY_ID").HasDatabaseName("IX_CATEGORY_ID").IsUnique(false);

		modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });
		modelBuilder.Entity<IdentityUserRole<string>>().HasKey(iur => new { iur.UserId, iur.RoleId });
		modelBuilder.Entity<IdentityUserToken<string>>().HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });
	}
}