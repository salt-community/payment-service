using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;


namespace PaymentService.Infrastructure.Persistence;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceLine> InvoiceLines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Invoice>(entity =>
        {

            entity.HasKey(i => i.Id);
            entity.Property(i => i.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(i => i.CustomerEmail).IsRequired().HasMaxLength(200);
            entity.Property(i => i.TotalAmount).HasColumnType("decimal(10,2)");
            entity.Property(i => i.Status).HasConversion<string>();
            entity.Property(i => i.CreatedAt).IsRequired();
            entity.Property(i => i.UpdatedAt).IsRequired();
            entity.HasMany(i => i.Lines).WithOne(l => l.Invoice).HasForeignKey(l => l.InvoiceId).OnDelete(DeleteBehavior.Cascade);

        });
        modelBuilder.Entity<InvoiceLine>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Name).IsRequired().HasMaxLength(200);
            entity.Property(l => l.ServiceType).IsRequired().HasMaxLength(100);
            entity.Property(l => l.UnitPrice).HasColumnType("decimal(10,2)");
            entity.Property(l => l.Amount).HasColumnType("decimal(10,2)");
        });
    }
}
