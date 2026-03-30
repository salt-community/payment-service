using PaymentService.Application.Interfaces;
using PaymentService.Application.Services;

namespace PaymentService.Api.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payment");

        group.MapPost("/{invoiceId}/pay", async (
        Guid invoiceId,
        PaymentRequest request,
        IPaymentService paymentService) =>
        {
            try
            {
                await paymentService.PayAsync(invoiceId, request.bookingId);

                return Results.Ok(new
                {
                    message = "Payment processed successfully"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

    }
}
public record PaymentRequest(Guid bookingId);

