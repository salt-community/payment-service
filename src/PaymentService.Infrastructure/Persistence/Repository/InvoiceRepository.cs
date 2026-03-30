using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Persistence;

public class InvoiceRepository(PaymentDbContext db)
{
    public async Task<Invoice?> GetByBookingIdAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default)
    {
        return await db.Invoices
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.BookingId == bookingId, cancellationToken);

    }
    // public Task<Invoice?> GetByIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    // Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    // Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);
}