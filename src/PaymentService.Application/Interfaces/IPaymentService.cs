using PaymentService.Application.Events;

namespace PaymentService.Application.Interfaces;

public interface IPaymentService
{
    Task HandleBookingCreatedAsync(BookingCreatedEvent message, CancellationToken cancellationToken = default);
    Task HandleWorkshopUpdatedAsync(WorkshopUpdatedEvent message, CancellationToken cancellationToken = default);
    Task HandleWorkshopCompletedAsync(WorkshopCompletedEvent message, CancellationToken cancellationToken = default);
    Task MarkAsPaidAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}