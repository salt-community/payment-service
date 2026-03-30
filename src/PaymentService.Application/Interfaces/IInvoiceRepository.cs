namespace PaymentService.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice> PayAsync(Guid invoiceId, Guid bookingId);
}