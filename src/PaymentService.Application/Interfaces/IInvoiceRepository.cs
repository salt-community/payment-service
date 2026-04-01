using PaymentService.Domain.Entities;

namespace PaymentService.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<IEnumerable<Invoice>> GetAllAsync(CancellationToken cancellationToken = default);
    void AddInvoiceLine(InvoiceLine line);
}