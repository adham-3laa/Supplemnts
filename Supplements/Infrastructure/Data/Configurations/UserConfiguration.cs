using Supplements.Core.Entities;
namespace Supplements.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
