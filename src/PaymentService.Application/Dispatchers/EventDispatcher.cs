using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Events;
using PaymentService.Application.Interfaces;

namespace PaymentService.Application.Dispatchers;

public class EventDispatcher(IPaymentService paymentService) : IEventDispatcher
{
    public async Task Dispatch(string topic, string json)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Console.WriteLine("Empty payload");
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            using var doc = JsonDocument.Parse(json);

            string? type = null;
            if (doc.RootElement.TryGetProperty("eventType", out var typeElement))
            {
                type = typeElement.GetString()?.ToLowerInvariant();
            }

            if (topic == "booking")
            {
                if (type == "created")
                {
                    var bookingEvent = JsonSerializer.Deserialize<BookingCreatedEvent>(json, options);
                    if (bookingEvent == null) return;

                    Console.WriteLine("Calling booking created service");
                    await paymentService.HandleBookingCreatedAsync(bookingEvent);
                    return;
                }

                Console.WriteLine($"No handler for topic '{topic}' and eventType '{type}'");
                return;
            }

            if (topic == "workshop")
            {
                if (type == "invoice-updated" || type == "workshop.invoice-updated")
                {
                    var workshopUpdateEvent = JsonSerializer.Deserialize<WorkshopInvoiceUpdatedEvent>(json, options);
                    if (workshopUpdateEvent == null) return;

                    Console.WriteLine("Calling workshop invoice updated service");
                    await paymentService.HandleWorkshopInvoiceUpdatedAsync(workshopUpdateEvent);
                    return;
                }

                if (type == "completed" || type == "workshop.completed")
                {
                    var workshopCompletedEvent = JsonSerializer.Deserialize<WorkshopCompletedEvent>(json, options);
                    if (workshopCompletedEvent == null) return;

                    Console.WriteLine("Calling workshop completed service");
                    await paymentService.HandleWorkshopCompletedAsync(workshopCompletedEvent);
                    return;
                }

                Console.WriteLine($"No handler for topic '{topic}' and eventType '{type}'");
                return;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"General Error: {e.Message}");
        }
    }
}