namespace PetCare.Payment.Application.Services
{
    using Microsoft.EntityFrameworkCore;
    using PetCare.Payment.Application.DTOs;
    using PetCare.Payment.Domain.Entities;
    using PetCare.Payment.Infrastructure.Gateways;
    using PetCare.Payment.Infrastructure.Persistence;
    using PetCare.Shared.DTOs.DTOs.Payment.Requests;
    using PetCare.Shared.DTOs.DTOs.Payment.Responses;
    using PetCares.Shared.DTOs.DTOs.Payment.Responses;
    using PetCareShared.DTOs.DTOs.Payment.Responses;

    public class PaymentService
    {
        private readonly PaymentGatewaySelector _gatewaySelector;
        private readonly PaymentDbContext _context;

        public PaymentService(PaymentGatewaySelector gatewaySelector, PaymentDbContext context)
        {
            _gatewaySelector = gatewaySelector;
            _context = context;
        }

        public async Task<PaymentGatewayResponse> ProcessPaymentAsync(string paymentMethodName, PaymentGatewayRequest request)
        {
            // Validar y resolver el método de pago
            var method = await _context.PaymentMethods
                .FirstOrDefaultAsync(m => m.Name == paymentMethodName);

            if (method == null)
            {
                return new PaymentGatewayResponse
                {
                    Success = false,
                    TransactionId = null,
                    Message = $"Método de pago '{paymentMethodName}' no está registrado."
                };
            }

            var gateway = _gatewaySelector.Resolve(paymentMethodName);
            var response = await gateway.ProcessPaymentAsync(request);

            var pay = new Pay
            {
                Id = Guid.NewGuid(),
                ReservationId = request.ReservationId,
                Amount = request.Amount,
                Currency = request.Currency,
                PaymentMethodId = method.Id,
                PaymentMethodToken = request.PaymentMethodToken,
                CustomerEmail = request.CustomerEmail,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                ExternalTransactionId = response.TransactionId,
                IsSuccessful = response.Success,
                PaymentStatusId = response.Success ? 2 : 3 // Completed or Failed
            };

            if (response.Success)
                pay.CompletedAt = DateTime.UtcNow;

            _context.Payments.Add(pay);
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<PaymentStatusResult> VerifyTransactionAsync(string paymentMethodName, string transactionId)
        {
            var gateway = _gatewaySelector.Resolve(paymentMethodName);
            var result = await gateway.VerifyTransactionAsync(transactionId);

            var pay = await _context.Payments
                .FirstOrDefaultAsync(p => p.ExternalTransactionId == transactionId);

            if (pay != null)
            {
                pay.PaymentStatusId = result.Status switch
                {
                    "Completed" => 2,
                    "Failed" => 3,
                    "Refunded" => 4,
                    _ => pay.PaymentStatusId
                };

                pay.IsSuccessful = result.Status == "Completed";
                pay.LastUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<RefundResult> RefundAsync(string paymentMethodName, string transactionId, decimal amount)
        {
            var gateway = _gatewaySelector.Resolve(paymentMethodName);
            var result = await gateway.RefundAsync(transactionId, amount);

            var pay = await _context.Payments
                .FirstOrDefaultAsync(p => p.ExternalTransactionId == transactionId);

            if (pay != null && result.Refunded)
            {
                pay.PaymentStatusId = 4; // Refunded
                pay.IsSuccessful = false;
                pay.LastUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<TransactionDetails?> GetTransactionDetailsAsync(string paymentMethodName, string transactionId)
        {
            var gateway = _gatewaySelector.Resolve(paymentMethodName);
            return await gateway.GetTransactionDetailsAsync(transactionId);
        }
    }
}
