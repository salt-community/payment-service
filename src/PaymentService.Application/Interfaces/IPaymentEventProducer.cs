namespace PaymentService.Application.Interfaces
{
    public interface IPaymentEventProducer
    {
        Task PublishPaymentPaidEventAsync<T>(T message);

    }
}