using Application.Domain.Enums;

namespace Application.Features.Payment;

public class PaymentUpdateStatusResult
{
    public PaymentRequestResult RequestResult { get; set; }
    public int OrderId { get; set; }

}
