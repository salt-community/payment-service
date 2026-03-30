using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Interfaces;

namespace PaymentService.Infrastructure.Persistence.Repository;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly PaymentDbContext _db;
    public InvoiceRepository(PaymentDbContext db)
    {
        _db = db;

    }

    public async Task<Invoice> PayAsync(Guid invoiceId, Guid bookingId)
    {
        var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId && i.BookingId == bookingId);
        if (invoice == null)
            throw new Exception("Invoice not found");
        invoice.Status = InvoiceStatus.Paid;
        invoice.UpdatedAt = DateTime.UtcNow;

        return invoice;
    }
}