namespace PaymentService.Application.Events;

public class WorkshopUpdatedEvent
{
    public Guid BookingId { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public List<WorkshopUpdatedPart> Parts { get; set; } = new();
}

public class WorkshopUpdatedPart
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}