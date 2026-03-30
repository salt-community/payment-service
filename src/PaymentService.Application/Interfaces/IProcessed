namespace PaymentService.Application.Interfaces;

public interface IProcessedEventRepository
{
    Task<bool> HasProcessedEventAsync(Guid eventId, CancellationToken ct = default);
    Task MarkAsProcessedAsync(Guid eventId, CancellationToken ct = default);
}