using PaymentService.Application.Events;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Interfaces;

public interface IPaymentService
{
    Task HandleBookingCreatedAsync(BookingCreatedEvent message, CancellationToken cancellationToken = default);
    Task HandleWorkshopUpdatedAsync(WorkshopUpdatedEvent message, CancellationToken cancellationToken = default);
    Task HandleWorkshopCompletedAsync(WorkshopCompletedEvent message, CancellationToken cancellationToken = default);
    Task MarkAsPaidAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Invoice>> GetAllInvoicesAsync(CancellationToken cancellationToken = default);
}