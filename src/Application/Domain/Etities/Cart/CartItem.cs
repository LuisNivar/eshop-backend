namespace Application.Domain.Entities.Cart;

public class CartItem
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public required string ImageUrl { get; set; }
    public int Quantity { get; set; }
}

