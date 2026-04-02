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
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("eventType", out var typeElement))
            {
                Console.WriteLine("No type found in Json");
            }

            string type = typeElement.GetString();

            if (topic == "booking" && type == "Created")
            {
                Console.WriteLine("Calling booking created service");
                Console.WriteLine(doc.RootElement.GetRawText());
                var bookingEvent = JsonSerializer.Deserialize<BookingCreatedEvent>(doc, options);
                await paymentService.HandleBookingCreatedAsync(bookingEvent);
            }
            else if (topic == "workshop")
            {
                Console.WriteLine("Calling workshop handlers");
                if (type == "update")
                {
                    Console.WriteLine(doc.RootElement.GetRawText());
                    var workshopUpdateEvent = JsonSerializer.Deserialize<WorkshopInvoiceUpdatedEvent>(doc, options);
                    await paymentService.HandleWorkshopInvoiceUpdatedAsync(workshopUpdateEvent);
                }
                else if (type == "completed")
                {
                    var workshopCompletedEvent = JsonSerializer.Deserialize<WorkshopCompletedEvent>(doc, options);
                    await paymentService.HandleWorkshopCompletedAsync(workshopCompletedEvent);
                }

            }
            else
            {
                Console.WriteLine("No topic found!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"General Error: {e.Message}");
        }
    }
}