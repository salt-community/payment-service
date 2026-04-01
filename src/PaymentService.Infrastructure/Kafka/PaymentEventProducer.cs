using System.Runtime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using PaymentService.Application.Events;
using PaymentService.Application.Interfaces;

namespace PaymentService.Infrastructure.Kafka;

public class PaymentEventProducer : IPaymentEventProducer
{
    private readonly IProducer<string, string> _producer;

    public PaymentEventProducer(ProducerConfig config)
    {

        _producer = new ProducerBuilder<string, string>(config).Build();

    }

    public async Task PublishPaymentStatusUpdatedAsync(PaymentStatusUpdatedEvent paymentEvent, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(paymentEvent);

        Console.WriteLine("Publishing payment.status.updated");
        Console.WriteLine($"BookingId: {paymentEvent.Data.BookingId}");
        Console.WriteLine($"PaymentId: {paymentEvent.Data.PaymentId}");
        Console.WriteLine($"Amount: {paymentEvent.Data.Amount}");
        Console.WriteLine($"Status: {paymentEvent.Data.Status}");
        Console.WriteLine($"Payload: {json}");

        var kafkaMessage = new Message<string, string>
        {
            Key = paymentEvent.EventId.ToString(),
            Value = json
        };

        var result = await _producer.ProduceAsync("payment.status.updated", kafkaMessage, cancellationToken);

        Console.WriteLine($"Produced to topic: {result.Topic}");
        Console.WriteLine($"Partition: {result.Partition}");
        Console.WriteLine($"Offset: {result.Offset}");
    }
}

