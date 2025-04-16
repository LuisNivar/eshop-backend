namespace Application.Features.Cart;
using CartEntity = Domain.Entities.Cart.Cart;

public class Cart
{
    //FIXME Clear Redis Manually: docker exec -it redis redis-cli FLUSHALL
    public required string UserId { get; set; }
    public List<CartItem> Items { get; set; } = [];


    public static Cart Map(CartEntity cart)
    {
        var newItems = cart.Items.Select(CartItem.Map).ToList();

        return new Cart
        {
            UserId = cart.UserId,
            Items = newItems
        };
    }

    public static CartEntity ToCart(Cart cart)
    {
        var newItems = cart.Items.Select(CartItem.ToCartItem).ToList();

        return new CartEntity
        {
            UserId = cart.UserId,
            Items = newItems
        };
    }
}
