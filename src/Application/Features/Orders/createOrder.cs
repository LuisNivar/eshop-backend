namespace Application.Features.Orders;

using Application.Domain.Enums;
using OrderEntity = Domain.Entities.Orders.Order;

public class CreateOrder
{
    public required DateTime OrderDate { get; set; }
    public required DateTime DueDate { get; set; }
    public required string ShippingDestination { get; set; }
    public required string UserId { get; set; }
    public required virtual List<CreateOrderItem> Items { get; set; } = [];
    public bool IsPaid { get; set; } = false;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public OrderEntity ToOrder()
    {
        var newItems = Items.Select(CreateOrderItem.ToOrderItem).ToList();

        return new OrderEntity
        {
            DueDate = DueDate,
            ShippingDestination = ShippingDestination,
            OrderDate = OrderDate,
            UserId = UserId,
            Items = newItems,
            Status = Status
        };
    }
}
