using PaymentService.Application.Interfaces;

namespace PaymentService.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentEventProducer _producer;
    private readonly IInvoiceRepository _invoiceRepo;
    public PaymentService(IPaymentEventProducer producer, IInvoiceRepository invoiceRepo)
    {
        _producer = producer;
        _invoiceRepo = invoiceRepo;
    }
    public async Task PayAsync(Guid invoiceId, Guid bookingId)
    {
        //update DB first
        var invoce = await _invoiceRepo.PayAsync(invoiceId, bookingId);

        var paymentEvent = new PaymentCompletedEvent
        {
            InvoiceId = invoiceId,
            BookingId = bookingId,
            Amount = invoce.TotalAmount,
            PaidAt = DateTime.UtcNow
        };
        await _producer.PublishPaymentPaidEventAsync(paymentEvent);
    }
}