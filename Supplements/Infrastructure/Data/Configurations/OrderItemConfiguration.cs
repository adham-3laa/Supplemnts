using Supplements.Core.Entities;
namespace Supplements.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(x => x.UnitPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.DiscountPercentage)
            .HasColumnType("decimal(5,2)");

        builder.Property(x => x.TotalPrice)
            .HasColumnType("decimal(18,2)");

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_OrderItem_Quantity",
            "[Quantity] > 0"));

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProductVariant)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
