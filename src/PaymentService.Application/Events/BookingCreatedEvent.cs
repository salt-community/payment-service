namespace PaymentService.Application.Events;

public class BookingCreatedEvent
{
    public Guid BookingId { get; set; }
    public BookingCustomerData Customer { get; set; } = new();
}

public class BookingCustomerData
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}