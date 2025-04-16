namespace Application.Features.Orders;

public static partial class OrderEndpoints
{
    record OrdersResponse(IEnumerable<Order> Result);
    record OrderResponse(Order Result);
}
