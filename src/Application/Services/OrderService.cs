using Application.Domain.Interfaces;
using OrderEntity = Application.Domain.Entities.Orders.Order;
using OrderItemEntity = Application.Domain.Entities.Orders.OrderItem;

using Application.Features.Orders;
using Application.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

//TODO: Order Services Interfaces

public class OrderService(ICartRepository repository, ApplicationDbContext db)
{
    private readonly ICartRepository _repository = repository;
    private readonly ApplicationDbContext _db = db;

    public async Task<OrderEntity> CreateOrderFromCartAsync(OrderFromCart orderFromCart)
    {
        string userId = orderFromCart.UserId;
        string shippingDestination = orderFromCart.ShippingDestination;
        int daysUntilDue = orderFromCart.DaysUntilDue;

        var cart = await _repository.GetCartAsync(userId);

        if (cart is null || cart.Items.Count == 0)
            throw new ArgumentException("Cart is empty or not exist.");

        // Mapear los items del carrito a items de orden
        var orderItems = cart.Items.Select(item => new OrderItemEntity
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
        }).ToList();

        // Crear la entidad de la orden
        var newOrder = new OrderEntity
        {
            UserId = cart.UserId,
            OrderDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(daysUntilDue),
            ShippingDestination = shippingDestination,
            Items = orderItems
        };

        _db.Orders.Add(newOrder);
        await _db.SaveChangesAsync();

        return newOrder;
    }
    public async Task<OrderEntity> UpdateOrderStatus(int id, OrderStatus status)
    {
        var orderToCancel = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (orderToCancel is null) throw new ArgumentException("Order not exist.");

        orderToCancel.Status = status;
        _db.Update(orderToCancel);
        await _db.SaveChangesAsync();

        return orderToCancel;
    }
}
