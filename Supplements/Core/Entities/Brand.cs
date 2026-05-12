namespace Supplements.Core.Entities;
public class Brand
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
