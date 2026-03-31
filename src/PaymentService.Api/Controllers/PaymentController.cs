using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;

namespace PaymentService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("{invoiceId}/pay")]
        public async Task<IActionResult> Pay(Guid invoiceId, CancellationToken ct)
        {
            if (invoiceId == Guid.Empty)
                return BadRequest("Invalid invoiceId");
            await _paymentService.MarkAsPaidAsync(invoiceId, ct);
            return Ok("Payment processed successfully");
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices(CancellationToken ct)
        {
            var invoices = await _paymentService.GetAllInvoicesAsync(ct);
            return Ok(invoices);
        }
        [HttpGet("{invoiceId}")]
        public async Task<ActionResult<Invoice>> GetInvoice(Guid invoiceId, CancellationToken ct)
        {
            var invoice = await _paymentService.GetInvoiceByIdAsync(invoiceId, ct);
            if (invoice == null)
                return NotFound("Invoice not found");
            return Ok(invoice);
        }

    }

}

