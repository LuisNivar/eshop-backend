namespace Application.Features.Catalog;

record ProductsResponse(IEnumerable<Product> Result);
record ProductResponse(Product Result);