using Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders;

public static partial class OrderEndpoints
{
    public static void AddOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/order");

        api.MapGet("/", GetOrders);
        api.MapPost("/", CreateOrder);
        api.MapPost("/cart/", CreateFromCart);
        api.MapGet("/{id:int}", GetOrderById);
        api.MapGet("/{userId}", GetOrdersByUserId);
        api.MapPut("/cancel/{id:int}", CancelOrder);
        api.MapPut("/expire/{id:int}", ExpireOrder);
    }

    private static async Task<Ok<OrdersResponse>> GetOrders(ApplicationDbContext dbContex)
    {
        var orders = await dbContex.Orders.Include(o => o.Items).ToListAsync();
        var mappedOrders = orders.Select(Order.Map);
        return TypedResults.Ok(new OrdersResponse(mappedOrders));
    }

    private static async Task<Results<Ok<OrderResponse>, NotFound>> GetOrderById(int id, ApplicationDbContext dbContext)
    {
        var order = await dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order is null) return TypedResults.NotFound();

        var mappedOrder = Order.Map(order!);
        return TypedResults.Ok(new OrderResponse(mappedOrder));
    }

    private static async Task<Results<Ok<OrdersResponse>, NotFound>> GetOrdersByUserId(string userId, ApplicationDbContext dbContext)
    {
        var filteredOrder = await dbContext.Orders.Include(o => o.Items).Where(o => o.UserId == userId).ToListAsync();
        if (filteredOrder is null) return TypedResults.NotFound();

        var mappedOrders = filteredOrder.Select(Order.Map).ToList();
        return TypedResults.Ok(new OrdersResponse(mappedOrders));
    }

    private static async Task<Ok<OrderResponse>> CreateOrder(CreateOrder createOrder, ApplicationDbContext dbContext)
    {
        var newOrder = createOrder.ToOrder();
        dbContext.Orders.Add(newOrder);
        await dbContext.SaveChangesAsync();
        var mappedOrder = Order.Map(newOrder);
        return TypedResults.Ok(new OrderResponse(mappedOrder));
    }
    private static async Task<Ok<OrderResponse>> CreateFromCart(OrderFromCart OrderFromCart, OrderService orderService)
    {
        var order = await orderService.CreateOrderFromCartAsync(OrderFromCart);
        var mappedOrder = Order.Map(order);
        return TypedResults.Ok(new OrderResponse(mappedOrder));
    }

    private static async Task<IResult> CancelOrder(int id, OrderService orderService)
    {
        var orderToCancel = await orderService.UpdateOrderStatus(id, Domain.Enums.OrderStatus.Cancelled);
        var mappedOrder = Order.Map(orderToCancel);
        return TypedResults.Ok(new OrderResponse(mappedOrder));
    }

    private static async Task<IResult> ExpireOrder(int id, OrderService orderService)
    {
        var orderToExpire = await orderService.UpdateOrderStatus(id, Domain.Enums.OrderStatus.Expired);
        var mappedOrder = Order.Map(orderToExpire);
        return TypedResults.Ok(new OrderResponse(mappedOrder));
    }
}
