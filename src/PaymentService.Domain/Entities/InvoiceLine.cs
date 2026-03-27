namespace PaymentService.Domain.Entities;

public class InvoiceLine
{
    int Id;
    int InvoiceId;
    string Name;
    decimal UnitPrice;
    decimal Amount;
    string ServiceType;
};