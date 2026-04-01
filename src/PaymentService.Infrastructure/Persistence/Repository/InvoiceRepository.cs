using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Persistence;

public class InvoiceRepository(PaymentDbContext db) : IInvoiceRepository
{
    public async Task<Invoice?> GetByBookingIdAsync(
        Guid bookingId,
        CancellationToken ct = default)
    {
        return await db.Invoices
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.BookingId == bookingId, ct);

    }
    public async Task<Invoice?> GetByIdAsync(
        Guid invoiceId,
        CancellationToken ct = default)
    {
        return await db.Invoices
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == invoiceId, ct);
    }
    public async Task AddAsync(Invoice invoice, CancellationToken ct = default)
    {
        await db.Invoices.AddAsync(invoice, ct);
        await db.SaveChangesAsync(ct);
    }
    public async Task UpdateAsync(Invoice invoice, CancellationToken ct = default)
    {
        await db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync(CancellationToken ct)
    {
        return await db.Invoices.Include(x => x.Lines).ToListAsync(ct);
    }

    public void AddInvoiceLine(InvoiceLine line)
    {
        db.InvoiceLines.Add(line);
    }
}