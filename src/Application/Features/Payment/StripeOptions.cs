namespace Application.Features.Payment;

public class StripeOptions
{
    public required string ApiKey { get; set; }
    public required string WebhookSecret { get; set; }
}
