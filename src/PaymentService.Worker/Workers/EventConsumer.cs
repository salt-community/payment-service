using Confluent.Kafka;
using PaymentService.Application.Dispatchers;

namespace PaymentService.Worker.Workers;

public class EventConsumer(ConsumerConfig consumerConfig, IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(new[] { "booking", "workshop" });

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    if (result != null)
                    {
                        using var scope = scopeFactory.CreateScope();

                        var dispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();
                        await dispatcher.Dispatch(result.Topic, result.Message.Value);
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine("Consume Error: {Reason}", e.Error.Reason);
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}
