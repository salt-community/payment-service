using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PaymentService.Worker.Workers;
using PaymentService.Application.Dispatchers;
using PaymentService.Application.Interfaces;
using PaymentService.Application.Services;
using PaymentService.Infrastructure.Persistence;
using PaymentService.Infrastructure.Kafka;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

var kafkaSection = builder.Configuration.GetSection("Kafka");

var consumerConfig = kafkaSection.Get<ConsumerConfig>();
// consumerConfig.GroupId = Guid.NewGuid().ToString(); // for development
builder.Services.AddSingleton(consumerConfig);

var producerConfig = kafkaSection.Get<ProducerConfig>();
builder.Services.AddSingleton(producerConfig);

builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddScoped<IPaymentService, PaymentService.Application.Services.PaymentService>();

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("defaultConnection")));

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPaymentEventProducer, PaymentEventProducer>();

builder.Services.AddHostedService<EventConsumer>();


var host = builder.Build();
host.Run();
