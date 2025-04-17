namespace Application.Domain.Entities.Cart;

public class Cart
{
    public required string UserId { get; set; }
    public List<CartItem> Items { get; set; } = [];
}
