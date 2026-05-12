using Supplements.Core.Entities;
namespace Supplements.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_CartItem_Quantity",
            "[Quantity] > 0"));

        builder.HasOne(x => x.Cart)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProductVariant)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
