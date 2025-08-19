using System.Threading.Tasks;
using PetCare.Payment.Application.DTOs;
using PetCare.Shared.DTOs.DTOs.Payment.Requests;
using PetCare.Shared.DTOs.DTOs.Payment.Responses;
using PetCares.Shared.DTOs.DTOs.Payment.Responses;
using PetCareShared.DTOs.DTOs.Payment.Responses;

namespace PetCare.Payment.Domain.Interfaces
{
    public interface IPaymentGatewayClient
    {
        /// <summary>
        /// Procesa un pago con los datos proporcionados.
        /// </summary>
        Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentGatewayRequest request);

        /// <summary>
        /// Verifica el estado de una transacción en la pasarela.
        /// </summary>
        Task<PaymentStatusResult> VerifyTransactionAsync(string transactionId);

        /// <summary>
        /// Solicita un reembolso para una transacción específica.
        /// </summary>
        Task<RefundResult> RefundAsync(string transactionId, decimal amount);

        /// <summary>
        /// Consulta el historial o detalles extendidos de una transacción.
        /// </summary>
        Task<TransactionDetails?> GetTransactionDetailsAsync(string transactionId);
    }

}
