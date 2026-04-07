namespace PaymentService.Application.Events;

public class WorkshopInvoiceUpdatedEvent
{
    public Guid BookingId { get; set; }
    public WorkshopProductData Product { get; set; } = new();
    public decimal Quantity { get; set; }
}

public class WorkshopProductData
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}