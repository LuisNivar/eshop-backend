
namespace Application.Features.Catalog;

using ProductEntity = Domain.Entities.Products.Product;

record CreateProduct(string Name, string Description, decimal Price, string ImageUrl)
{
    public ProductEntity ToProduct() => new()
    {
        Name = Name,
        Description = Description,
        Price = Price,
        ImageUrl = ImageUrl
    };
}
