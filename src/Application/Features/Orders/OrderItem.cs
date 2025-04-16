namespace Application.Features.Orders;
using OrderItemEntity = Domain.Entities.Orders.OrderItem;

public class OrderItem
{
    public required int ProductId { get; set; }
    public int Quantity { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }

    public static OrderItem Map(OrderItemEntity orderItem)
    {
        return new OrderItem
        {
            ProductId = orderItem.ProductId,
            ProductName = orderItem.ProductName,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice
        };
    }


}
