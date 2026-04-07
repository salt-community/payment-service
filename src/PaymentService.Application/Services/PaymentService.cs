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
        var existingInvoice = await _invoiceRepository
            .GetByBookingIdAsync(message.BookingId, cancellationToken);

        if (existingInvoice is not null)
            return;

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            BookingId = message.BookingId,
            CustomerName = message.Customer.Name,
            CustomerEmail = message.Customer.Email,
            Status = InvoiceStatus.Pending,
            TotalAmount = 0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _invoiceRepository.AddAsync(invoice, cancellationToken);
    }

    public async Task HandleWorkshopInvoiceUpdatedAsync(
    WorkshopInvoiceUpdatedEvent message,
    CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository
            .GetByBookingIdAsync(message.BookingId, cancellationToken);

        if (invoice == null)
        {
            Console.WriteLine($"No invoice found for workshop invoice update. BookingId: {message.BookingId}");
            return;
        }

        var productName = message.Product?.Name;
        var unitPrice = message.Product?.Price ?? 0m;
        var quantity = message.Quantity;

        if (string.IsNullOrWhiteSpace(productName))
        {
            Console.WriteLine("No product name in workshop invoice update");
            return;
        }

        var existingLine = invoice.Lines
            .FirstOrDefault(x => x.Name == productName);

        if (existingLine == null)
        {
            var newLine = new InvoiceLine
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                Name = productName,
                UnitPrice = unitPrice,
                Amount = quantity,
                ServiceType = string.Empty
            };

            invoice.Lines.Add(newLine);
            _invoiceRepository.AddInvoiceLine(newLine);
        }
        else
        {
            existingLine.UnitPrice = unitPrice;
            existingLine.Amount = quantity;
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

        if (invoice.Status == InvoiceStatus.Invoiced)
            return;

        invoice.Status = InvoiceStatus.Invoiced;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);

        await PublishPaymentStatusUpdatedAsync(invoice, cancellationToken);
    }

    public async Task MarkAsPaidAsync(
    Guid invoiceId,
    CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);

        if (invoice == null)
            return;

        invoice.Status = InvoiceStatus.Paid;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);

        await PublishPaymentStatusUpdatedAsync(invoice, cancellationToken);
    }

    public async Task MarkAsFailedAsync(
    Guid invoiceId,
    CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);

        if (invoice == null)
            return;

        invoice.Status = InvoiceStatus.Failed;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);

        await PublishPaymentStatusUpdatedAsync(invoice, cancellationToken);
    }

    private async Task PublishPaymentStatusUpdatedAsync(
    Invoice invoice,
    CancellationToken cancellationToken = default)
    {
        var paymentEvent = new PaymentStatusUpdatedEvent
        {
            EventId = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Data = new PaymentStatusUpdatedData
            {
                BookingId = invoice.BookingId,
                PaymentId = invoice.Id,
                Amount = invoice.TotalAmount,
                Status = invoice.Status.ToString()
            }
        };

        await _eventProducer.PublishPaymentStatusUpdatedAsync(paymentEvent, cancellationToken);
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        return await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
    }
    public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _invoiceRepository.GetAllAsync(cancellationToken);
    }
}