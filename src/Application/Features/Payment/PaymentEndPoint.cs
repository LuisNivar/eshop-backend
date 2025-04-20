using Application.Domain.Enums;
using Application.Domain.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Features.Payment;

public static class PaymentEndpoint
{
    public static void AddPaymentEndpoint(this IEndpointRouteBuilder api)
    {
        api.MapPost("/pay/{orderId:int}", PayOrder);
        api.MapPost("/webhook", UpdateStatus);
    }

    private static async Task<Results<Ok<PaymentResult>, BadRequest>> PayOrder
    (
        int orderId, ApplicationDbContext dbContext,
        PaymentService paymentService)
    {
        var _domain = "http://localhost:5173"; //Supposed frontend
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null || order.IsPaid)
            return TypedResults.BadRequest();

        var result = await paymentService.PayOrder(order, _domain);
        return TypedResults.Ok(result);
    }

    public static async Task<IResult> UpdateStatus(
        HttpRequest request,
        ApplicationDbContext db,
        IPayment paymentService,
        IOptions<StripeOptions> options
        )
    {
        var json = await new StreamReader(request.Body).ReadToEndAsync();
        Console.WriteLine(new { json });
        var stripeSignature = request.Headers["Stripe-Signature"];
        var endpointSecret = options.Value.WebhookSecret;

        var result = paymentService.UpdateStatus(json, stripeSignature!, endpointSecret);

        if (result.RequestResult == PaymentRequestResult.BadRequest)
            return TypedResults.BadRequest();

        if (result.RequestResult == PaymentRequestResult.NotFound)
            return TypedResults.NotFound();

        int orderId = result.OrderId;
        var order = await db.Orders.FindAsync(orderId);

        if (order is null)
            return Results.NotFound("Order not found!");

        order.IsPaid = true;
        order.Status = OrderStatus.Completed;

        db.Orders.Update(order);
        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
