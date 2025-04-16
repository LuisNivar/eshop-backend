using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Catalog;

public static class CatalogEndpoints
{
    public static void AddCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/products");

        api.MapGet("/", GetProducts);
        api.MapPost("/", CreateProduct);
        api.MapGet("/{id:int}", GetProductById);
        api.MapDelete("/{id:int}", DeleteProduct);
    }

    private static async Task<IResult> DeleteProduct(ApplicationDbContext dbContex, int id)
    {
        var productToRemove = await dbContex.Products.FindAsync(id);
        if (productToRemove is null) return Results.NotFound();

        dbContex.Products.Remove(productToRemove);
        dbContex.SaveChanges();
        return Results.NoContent();
    }

    private static async Task<Results<Ok<ProductResponse>, NotFound>> GetProductById(int id, ApplicationDbContext dbContext)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product is null) return TypedResults.NotFound();
        var mappedProduct = Product.Map(product);
        return TypedResults.Ok(new ProductResponse(mappedProduct));
    }

    private static async Task<Ok<ProductsResponse>> GetProducts(ApplicationDbContext dbContext)
    {
        var produtcs = await dbContext.Products.ToListAsync();
        var mappedProducts = produtcs.Select(Product.Map);
        return TypedResults.Ok(new ProductsResponse(mappedProducts));
    }

    private static async Task<Ok<ProductResponse>> CreateProduct(CreateProduct newProduct, ApplicationDbContext dbContext)
    {
        var product = newProduct.ToProduct();

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        var mappedProduct = Product.Map(product);

        return TypedResults.Ok(new ProductResponse(mappedProduct));
    }
}
