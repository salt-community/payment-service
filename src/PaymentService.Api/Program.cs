using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using PaymentService.Application.Interfaces;
using PaymentService.Application.Services;
using PaymentService.Infrastructure.Kafka;
using PaymentService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService.Application.Services.PaymentService>();

var kafkaSection = builder.Configuration.GetSection("Kafka");

var producerConfig = kafkaSection.Get<ProducerConfig>();
builder.Services.AddSingleton(producerConfig);

builder.Services.AddSingleton<IPaymentEventProducer, PaymentEventProducer>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();