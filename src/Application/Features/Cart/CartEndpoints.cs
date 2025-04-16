using Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.Cart;

public static class CartEndpoints
{
    public static void AddCartEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/cart");

        api.MapGet("/{userId}", GetCartByUserId);
        // api.MapPost("/", CreateCart);
        api.MapPost("/items", AddItemsCart);
        api.MapPut("/items", UpdateQuantityItemsCart);
        api.MapDelete("/items", RemoveItemAsync);
        api.MapDelete("/{userId}", RemoveCartByCustomerId);
    }

    private static async Task<Results<Ok<CartResponse>, NotFound>> GetCartByUserId(string userId, CartService cartService)
    {
        var cart = await cartService.GetAsync(userId);
        if (cart is null) return TypedResults.NotFound();
        var MappedCart = Cart.Map(cart!);
        return TypedResults.Ok(new CartResponse(MappedCart));
    }

    // private static async Task<Ok<CartResponse>> CreateCart(Cart cart, CartService cartService)
    // {
    //     var newCart = await cartService.CreateAsync(Cart.ToCart(cart));
    //     var MappedCart = Cart.Map(newCart!);
    //     return TypedResults.Ok(new CartResponse(MappedCart));
    // }

    private static async Task<Ok<CartResponse>> AddItemsCart(string userId, int productId, int quantity, CartService cartService)
    {
        var newCart = await cartService.AddItemAsync(userId, productId, quantity);
        var MappedCart = Cart.Map(newCart!);
        return TypedResults.Ok(new CartResponse(MappedCart));
    }

    private static async Task<Ok<CartResponse>> UpdateQuantityItemsCart(string userId, int productId, int quantity, CartService cartService)
    {
        var newCart = await cartService.UpdateQuantityItemAsync(userId, productId, quantity);
        var MappedCart = Cart.Map(newCart!);
        return TypedResults.Ok(new CartResponse(MappedCart));
    }

    private static async Task<IResult> RemoveItemAsync(string userId, int productId, CartService cartService)
    {
        var deleted = await cartService.RemoveItemAsync(userId, productId);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> RemoveCartByCustomerId(string userId, CartService cartService)
    {
        var deleted = await cartService.DeleteAsync(userId);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}