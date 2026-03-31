namespace PaymentService.Application.Events;

public class WorkshopCompletedEvent
{
    public Guid BookingId { get; set; }
}