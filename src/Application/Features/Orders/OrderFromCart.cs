namespace Application.Features.Orders;

public record OrderFromCart(string UserId, string ShippingDestination, int DaysUntilDue);
