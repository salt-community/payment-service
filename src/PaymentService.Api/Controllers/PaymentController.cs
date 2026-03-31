using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Interfaces;

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
            return Ok("Payment processed Sucessfully");
        }

    }

}