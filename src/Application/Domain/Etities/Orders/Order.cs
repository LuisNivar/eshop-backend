using Application.Domain.Enums;

namespace Application.Domain.Entities.Orders;

public class Order
{
    public int Id { get; set; }
    public required DateTime OrderDate { get; set; }
    public required DateTime DueDate { get; set; }
    public required string ShippingDestination { get; set; }
    public required string UserId { get; set; }
    public required virtual List<OrderItem> Items { get; set; } = [];
    public bool IsPaid { get; set; } = false;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}

