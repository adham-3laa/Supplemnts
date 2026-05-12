using Supplements.Core.Entities;
namespace Supplements.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.TotalPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.ShippingCost)
            .HasColumnType("decimal(18,2)");

        // User → Orders: NO CASCADE (avoid multiple cascade paths)
        builder.HasOne(x => x.User)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Address → Orders: NO CASCADE (avoid cycle: User→Address→Order and User→Order)
        builder.HasOne(x => x.Address)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.AddressId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
