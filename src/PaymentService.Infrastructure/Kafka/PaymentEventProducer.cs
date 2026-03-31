using System.Runtime;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using PaymentService.Application.Events;
using PaymentService.Application.Interfaces;

namespace PaymentService.Infrastructure.Kafka;

public class PaymentEventProducer : IPaymentEventProducer
{
    private readonly IConfiguration _config;
    private readonly IProducer<string, string> _producer;

    public PaymentEventProducer(IConfiguration config)
    {
        _config = config;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            SecurityProtocol = SecurityProtocol.Ssl,
            SslCaLocation = "certs/ca.pem",
            SslCertificateLocation = "certs/service.cert",
            SslKeyLocation = "certs/service.key",
            Acks = Acks.All
        };
        _producer = new ProducerBuilder<string, string>(producerConfig).Build();

    }

    public Task PublishPaymentStatusUpdatedAsync(PaymentStatusUpdatedEvent paymentEvent, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(paymentEvent);
        var kafkaMessage = new Message<string, string>
        {
            Key = paymentEvent.EventId.ToString(),
            Value = json
        };
        return _producer.ProduceAsync("payments", kafkaMessage);
    }
}

