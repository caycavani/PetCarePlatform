using PetCare.Payment.Application.DTOs;
using PetCare.Shared.DTOs.DTOs.Payment.Requests;
using PetCare.Shared.DTOs.DTOs.Payment.Responses;
using PetCares.Shared.DTOs.DTOs.Payment.Responses;
using PetCareShared.DTOs.DTOs.Payment.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.Payment.Infrastructure.Gateways.Wompi
{
    public class WompiGatewayClient : IPaymentGatewayClient
    {
        public async Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentGatewayRequest request)
        {
            return await Task.FromResult(new PaymentGatewayResponse
            {
                Success = true,
                TransactionId = Guid.NewGuid().ToString(),
                Message = "Pago procesado exitosamente con Wompi"
            });
        }

        public async Task<PaymentStatusResult> VerifyTransactionAsync(string transactionId)
        {
            return await Task.FromResult(new PaymentStatusResult
            {
                TransactionId = transactionId,
                Status = "Approved",
                IsSuccessful = true,
                LastUpdated = DateTime.UtcNow
            });
        }

        public async Task<RefundResult> RefundAsync(string transactionId, decimal amount)
        {
            return await Task.FromResult(new RefundResult
            {
                TransactionId = transactionId,
                Refunded = true,
                Message = $"Reembolso de {amount:C} procesado por Wompi",
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task<TransactionDetails> GetTransactionDetailsAsync(string transactionId)
        {
            return await Task.FromResult(new TransactionDetails
            {
                TransactionId = transactionId,
                Amount = 85000,
                Currency = "COP",
                Status = "Approved",
                CreatedAt = DateTime.UtcNow.AddMinutes(-15),
                CompletedAt = DateTime.UtcNow,
                GatewayName = "Wompi"
            });
        }
    }

}
