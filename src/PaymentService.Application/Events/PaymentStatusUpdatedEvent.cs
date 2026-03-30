namespace PaymentService.Application.Events;

public class PaymentStatusUpdatedEvent
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = "payment.status.updated";
    public DateTime Timestamp { get; set; }
    public PaymentStatusUpdatedData Data { get; set; } = new();
}

public class PaymentStatusUpdatedData
{
    public Guid BookingId { get; set; }
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}