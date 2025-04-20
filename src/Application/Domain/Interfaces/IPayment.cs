using Application.Domain.Entities.Orders;
using Application.Features.Payment;

namespace Application.Domain.Interfaces;

public interface IPayment
{
    public Task<PaymentResult> PayOrder(Order order, string domain);
    public PaymentUpdateStatusResult UpdateStatus(string json, string stripeSignature, string endpointSecret);
}
