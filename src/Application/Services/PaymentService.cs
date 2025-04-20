using Application.Domain.Entities.Orders;
using Application.Domain.Enums;
using Application.Domain.Interfaces;
using Application.Features.Payment;
using Stripe;
using Stripe.Checkout;

namespace Application.Services;

public class PaymentService : IPayment
{
    public async Task<PaymentResult> PayOrder(Order order, string domain)
    {
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
            PaymentMethodTypes = ["card"],
            Mode = "payment",
            SuccessUrl = $"{domain}/success?orderId={order.Id}",
            CancelUrl = $"{domain}/cancel",
            LineItems = lineItems
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return new PaymentResult
        {
            Id = session.Id,
            Url = session.Url
        };
    }

    public PaymentUpdateStatusResult UpdateStatus(string json, string stripeSignature, string endpointSecret)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);

            if (stripeEvent.Type != "checkout.session.completed")
            {
                return new()
                {
                    RequestResult = PaymentRequestResult.BadRequest,
                    OrderId = 0
                };
            }

            var session = stripeEvent.Data.Object as Session;

            if (session?.Metadata is null)
            {
                return new()
                {
                    RequestResult = PaymentRequestResult.BadRequest,
                    OrderId = 0
                };
            }

            var orderId = int.Parse(session?.SuccessUrl?.Split("orderId=")?.Last() ?? "0");

            if (orderId == 0)
            {
                return new()
                {
                    RequestResult = PaymentRequestResult.NotFound,
                    OrderId = 0
                };
            }
            return new()
            {
                RequestResult = PaymentRequestResult.Ok,
                OrderId = orderId
            };
        }
        catch (Exception e)
        {
            throw new Exception($"Webhook error: {e.Message}");
        }

    }
}
