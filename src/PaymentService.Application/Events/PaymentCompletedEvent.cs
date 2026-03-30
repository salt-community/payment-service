namespace PaymentService.Application.Interfaces;

public class PaymentCompletedEvent
{
    public Guid InvoiceId { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }


}