using Application.Domain.Enums;
using OrderEntity = Application.Domain.Entities.Orders.Order;

namespace Application.Features.Orders;

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

    public static Order Map(OrderEntity order)
    {
        var newItems = order.Items.Select(OrderItem.Map).ToList();

        return new Order
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            DueDate = order.DueDate,
            ShippingDestination = order.ShippingDestination,
            UserId = order.UserId,
            Items = newItems,
            Status = order.Status,
            IsPaid = order.IsPaid
        };
    }
}