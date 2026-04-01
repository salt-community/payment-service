using System.Text.Json;
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
                    Console.WriteLine("Calling booking created service");

                    var bookingEvent = JsonSerializer.Deserialize<BookingCreatedEvent>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    if (bookingEvent == null)
                    {
                        Console.WriteLine("Failed to deserialize BookingCreatedEvent");
                        return;
                    }

                    Console.WriteLine($"BookingCreated BookingId: {bookingEvent.BookingId}");
                    Console.WriteLine($"CustomerName: {bookingEvent.Customer?.Name}");
                    Console.WriteLine($"CustomerEmail: {bookingEvent.Customer?.Email}");

                    await paymentService.HandleBookingCreatedAsync(bookingEvent);
                    return;
                }

                Console.WriteLine($"No booking handler for eventType '{type}'");
                return;
            }

            if (topic == "workshop")
            {
                if (type == "completed")
                {
                    Console.WriteLine("Calling workshop completed service");

                    var completedEvent = JsonSerializer.Deserialize<WorkshopCompletedEvent>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    if (completedEvent == null)
                    {
                        Console.WriteLine("Failed to deserialize WorkshopCompletedEvent");
                        return;
                    }

                    await paymentService.HandleWorkshopCompletedAsync(completedEvent);
                    return;
                }

                var isUpdate =
                    type == "update" ||
                    doc.RootElement.TryGetProperty("ServiceType", out _) ||
                    doc.RootElement.TryGetProperty("Parts", out _);

                if (isUpdate)
                {
                    Console.WriteLine("Calling workshop updated service");

                    var updateEvent = JsonSerializer.Deserialize<WorkshopUpdatedEvent>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    if (updateEvent == null)
                    {
                        Console.WriteLine("Failed to deserialize WorkshopUpdatedEvent");
                        return;
                    }

                    Console.WriteLine($"WorkshopUpdated BookingId: {updateEvent.BookingId}");
                    Console.WriteLine($"ServiceType: {updateEvent.ServiceType}");
                    Console.WriteLine($"Parts count: {updateEvent.Parts?.Count ?? 0}");

                    await paymentService.HandleWorkshopUpdatedAsync(updateEvent);
                    return;
                }

                Console.WriteLine($"No workshop handler for eventType '{type}'");
                return;
            }

            Console.WriteLine($"No handler for topic '{topic}'");
        }
        catch (Exception e)
        {
            Console.WriteLine($"General Error: {e.Message}");
        }
    }
}