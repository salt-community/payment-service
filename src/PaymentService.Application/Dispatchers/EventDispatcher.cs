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
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("eventType", out var typeElement))
            {
                Console.WriteLine("No type found in Json");
            }

            string type = typeElement.GetString();

            if (topic == "booking" && type == "Created")
            {
                Console.WriteLine("Calling booking created service");
                var bookingEvent = JsonSerializer.Deserialize<BookingCreatedEvent>(doc);
                await paymentService.HandleBookingCreatedAsync(bookingEvent);
            }
            else if (topic == "workshop")
            {
                Console.WriteLine("Should call workshop handlers");
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