using PaymentService.Application.Events;

namespace PaymentService.Application.Interfaces;

public interface IPaymentEventProducer
{
    Task PublishPaymentStatusUpdatedAsync(
        PaymentStatusUpdatedEvent paymentEvent,
        CancellationToken cancellationToken = default);
}