using Microsoft.AspNetCore.Mvc;
using PetCare.Payment.Application.DTOs;
using PetCare.Payment.Application.Services;
using PetCare.Shared.DTOs.DTOs.Payment.Requests;

namespace PetCare.Payment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Procesa un pago PSE u otro método configurado.
        /// </summary>
        [HttpPost("{paymentMethodName}")]
        public async Task<IActionResult> ProcessPayment(string paymentMethodName, [FromBody] PaymentGatewayRequest request)
        {
            var result = await _paymentService.ProcessPaymentAsync(paymentMethodName, request);

            if (result == null)
                return BadRequest($"Método de pago '{paymentMethodName}' no está registrado.");

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Verifica el estado de una transacción por ID.
        /// </summary>
        [HttpGet("{paymentMethodName}/verify/{transactionId}")]
        public async Task<IActionResult> VerifyTransaction(string paymentMethodName, string transactionId)
        {
            var result = await _paymentService.VerifyTransactionAsync(paymentMethodName, transactionId);

            if (result == null)
                return NotFound($"No se encontró la transacción '{transactionId}' para el método '{paymentMethodName}'.");

            return Ok(result);
        }

        /// <summary>
        /// Solicita reembolso parcial o total de una transacción.
        /// </summary>
        [HttpPost("{paymentMethodName}/refund/{transactionId}")]
        public async Task<IActionResult> Refund(string paymentMethodName, string transactionId, [FromQuery] decimal amount)
        {
            var result = await _paymentService.RefundAsync(paymentMethodName, transactionId, amount);

            if (result == null)
                return BadRequest($"No se pudo procesar el reembolso para '{transactionId}' con el método '{paymentMethodName}'.");

            return result.Refunded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtiene los detalles completos de una transacción.
        /// </summary>
        [HttpGet("{paymentMethodName}/details/{transactionId}")]
        public async Task<IActionResult> GetTransactionDetails(string paymentMethodName, string transactionId)
        {
            var result = await _paymentService.GetTransactionDetailsAsync(paymentMethodName, transactionId);

            return result is not null ? Ok(result) : NotFound($"No se encontraron detalles para la transacción '{transactionId}' con el método '{paymentMethodName}'.");
        }
    }
}
