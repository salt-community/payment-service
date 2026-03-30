using System.Runtime;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
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
    public async Task PublishPaymentPaidEventAsync<T>(T message)
    {
        var eventEnvelop = new
        {
            eventId = Guid.NewGuid(),
            eventType = "payment.paid",
            timestamp = DateTime.UtcNow,
            data = message
        };


        var json = JsonSerializer.Serialize(eventEnvelop);
        var kafkaMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = json
        };
        await _producer.ProduceAsync("payments", kafkaMessage);


    }
}

