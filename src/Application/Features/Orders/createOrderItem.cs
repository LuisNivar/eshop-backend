namespace Application.Features.Orders;

using OrderItemEntity = Domain.Entities.Orders.OrderItem;

public class CreateOrderItem
{
    public required int ProductId { get; set; }
    public int Quantity { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }

    public static OrderItemEntity ToOrderItem(CreateOrderItem orderItem)
    {
        return new OrderItemEntity
        {
            ProductId = orderItem.ProductId,
            ProductName = orderItem.ProductName,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice
        };
    }
}