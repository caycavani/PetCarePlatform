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

namespace PetCare.Payment.Infrastructure.Gateways
{
    public interface IPaymentGatewayClient
    {
        Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentGatewayRequest request);
        Task<PaymentStatusResult> VerifyTransactionAsync(string transactionId);
        Task<RefundResult> RefundAsync(string transactionId, decimal amount);
        Task<TransactionDetails?> GetTransactionDetailsAsync(string transactionId);
    }

}
