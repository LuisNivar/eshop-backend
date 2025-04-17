using System.Data;
using Application.Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Application.Features.Payment;

public static class PaymentEndpoint
{
    //TODO: Move to service + interface
    public static void AddPaymentEndpoint(this IEndpointRouteBuilder api)
    {
        api.MapPost("/pay/{orderId:int}", PayOrder);
        api.MapPost("/webhook", UpdateStatus);
    }

    private static async Task<IResult> PayOrder(int orderId, ApplicationDbContext dbContext, HttpContext httpContext)
    {
        var _domain = "http/frontend-fake";
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null || order.IsPaid)
            return Results.BadRequest("Order invalid!");

        var lineItems = order.Items.Select(item => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmountDecimal = item.UnitPrice * 100, // convertir a centavos
                Currency = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = item.ProductName
                }
            },
            Quantity = item.Quantity
        }).ToList();

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "payment",
            SuccessUrl = $"{_domain}/success?orderId={order.Id}",
            CancelUrl = $"{_domain}/cancel",
            LineItems = lineItems
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return Results.Ok(new { sessionId = session.Id, url = session.Url });
    }

    public static async Task<IResult> UpdateStatus(HttpRequest request, ApplicationDbContext db, IOptions<StripeOptions> options)
    {
        var json = await new StreamReader(request.Body).ReadToEndAsync();
        var stripeSignature = request.Headers["Stripe-Signature"];

        var endpointSecret = options.Value.WebhookSecret;

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);
        }
        catch (Exception e)
        {
            return Results.BadRequest($"Webhook error: {e.Message}");
        }

        //FIXME: Debug
        Console.WriteLine(new { stripeEvent.Type });

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;
            var orderId = int.Parse(session?.SuccessUrl?.Split("orderId=")?.Last() ?? "0");

            var order = await db.Orders.FindAsync(orderId);
            if (order is not null)
            {
                order.IsPaid = true;
                order.Status = OrderStatus.Completed;
                await db.SaveChangesAsync();
            }
        }

        return Results.Ok();
    }
}
