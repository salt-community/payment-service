using PaymentService.Application.Events;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;

namespace PaymentService.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentEventProducer _eventProducer;

    public PaymentService(
        IInvoiceRepository invoiceRepository,
        IPaymentEventProducer eventProducer)
    {
        _invoiceRepository = invoiceRepository;
        _eventProducer = eventProducer;
    }

    public async Task HandleBookingCreatedAsync(
        BookingCreatedEvent message,
        CancellationToken cancellationToken = default)
    {
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            BookingId = message.BookingId,
            Status = InvoiceStatus.Pending,
            TotalAmount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _invoiceRepository.AddAsync(invoice, cancellationToken);
    }

    public async Task HandleWorkshopUpdatedAsync(
        WorkshopUpdatedEvent message,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository
            .GetByBookingIdAsync(message.BookingId, cancellationToken);

        if (invoice == null)
            return;

        var existingLine = invoice.Lines
            .FirstOrDefault(x => x.ServiceType == message.ServiceType && x.Name == message.Name);

        if (existingLine == null)
        {
            invoice.Lines.Add(new InvoiceLine
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                Name = message.Name,
                UnitPrice = message.Price,
                Amount = message.Amount,
                ServiceType = message.ServiceType
            });
        }
        else
        {
            existingLine.UnitPrice = message.Price;
            existingLine.Amount = message.Amount;
        }

        invoice.TotalAmount = invoice.Lines.Sum(x => x.UnitPrice * x.Amount);
        invoice.UpdatedAt = DateTime.UtcNow;

        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
    }

    public async Task HandleWorkshopCompletedAsync(
        WorkshopCompletedEvent message,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository
            .GetByBookingIdAsync(message.BookingId, cancellationToken);

        if (invoice == null)
            return;

        invoice.Status = InvoiceStatus.Invoiced;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);

        var paymentEvent = new PaymentStatusUpdatedEvent
        {
            EventId = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Data = new PaymentStatusUpdatedData
            {
                BookingId = invoice.BookingId,
                InvoiceId = invoice.Id,
                Amount = invoice.TotalAmount,
                Status = invoice.Status.ToString()
            }
        };

        await _eventProducer.PublishPaymentStatusUpdatedAsync(paymentEvent, cancellationToken);
    }
}