using CartItemEntity = Application.Domain.Entities.Cart.CartItem;

namespace Application.Features.Cart;

public class CartItem
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public required string ImageUrl { get; set; }
    public int Quantity { get; set; }

    public static CartItem Map(CartItemEntity cartItem)
    {
        return new CartItem
        {
            ProductId = cartItem.ProductId,
            Quantity = cartItem.Quantity,
            ImageUrl = cartItem.ImageUrl,
            ProductName = cartItem.ProductName,
            UnitPrice = cartItem.UnitPrice
        };
    }

    public static CartItemEntity ToCartItem(CartItem cartItem)
    {
        return new CartItemEntity
        {
            ProductId = cartItem.ProductId,
            Quantity = cartItem.Quantity,
            ImageUrl = cartItem.ImageUrl,
            ProductName = cartItem.ProductName,
            UnitPrice = cartItem.UnitPrice
        };
    }
}
