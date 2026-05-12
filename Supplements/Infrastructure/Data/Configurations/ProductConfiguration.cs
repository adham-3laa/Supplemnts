using Supplements.Core.Entities;
namespace Supplements.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.DiscountPercentage)
            .HasColumnType("decimal(5,2)");

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Product_Stock",
            "[StockQuantity] >= 0"));

        builder.HasOne(x => x.Seller)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId);

        builder.HasOne(x => x.Brand)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.BrandId);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
