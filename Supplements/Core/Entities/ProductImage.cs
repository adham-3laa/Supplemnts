namespace Supplements.Core.Entities;
public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;
}
