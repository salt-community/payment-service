namespace PaymentService.Application.Services;

public interface IPaymentService
{
    Task PayAsync(Guid invoiceId, Guid bookingId);
}