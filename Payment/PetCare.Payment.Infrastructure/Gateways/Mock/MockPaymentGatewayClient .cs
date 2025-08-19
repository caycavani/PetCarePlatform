using PetCare.Payment.Application.DTOs;
using PetCare.Shared.DTOs.DTOs.Payment.Requests;
using PetCare.Shared.DTOs.DTOs.Payment.Responses;
using PetCares.Shared.DTOs.DTOs.Payment.Responses;
using PetCareShared.DTOs.DTOs.Payment.Responses;
using System;
using System.Threading.Tasks;

namespace PetCare.Payment.Infrastructure.Gateways.Mock
{
    public class MockPaymentGatewayClient : IPaymentGatewayClient
    {
        public Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentGatewayRequest request)
            => Task.FromResult(new PaymentGatewayResponse
            {
                Success = true,
                TransactionId = Guid.NewGuid().ToString(),
                Message = "Pago simulado exitosamente"
            });

        public Task<PaymentStatusResult> VerifyTransactionAsync(string transactionId)
            => Task.FromResult(new PaymentStatusResult
            {
                TransactionId = transactionId,
                Status = "Completed",
                IsSuccessful = true,
                LastUpdated = DateTime.UtcNow
            });

        public Task<RefundResult> RefundAsync(string transactionId, decimal amount)
            => Task.FromResult(new RefundResult
            {
                TransactionId = transactionId,
                Refunded = true,
                Message = "Reembolso simulado exitosamente",
                Timestamp = DateTime.UtcNow
            });

        public Task<TransactionDetails?> GetTransactionDetailsAsync(string transactionId)
            => Task.FromResult<TransactionDetails?>(new TransactionDetails
            {
                TransactionId = transactionId,
                Amount = 100000,
                Currency = "COP",
                Status = "Completed",
                CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                CompletedAt = DateTime.UtcNow,
                GatewayName = "Mock"
            });
    }
}
