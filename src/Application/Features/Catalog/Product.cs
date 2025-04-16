using EntityProduct = Application.Domain.Entities.Products.Product;

namespace Application.Features.Catalog;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
    public decimal Price { get; set; }

    public static Product Map(EntityProduct product)
    {
        return new Product
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            Price = product.Price
        };
    }
}
