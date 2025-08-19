namespace PetCare.Payment.Domain.Entities
{
    using System;

    public class Pay
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public decimal Amount { get; set; }

        public int PaymentMethodId { get; set; }
        public PaymentMethod Method { get; set; }

        public int PaymentStatusId { get; set; }
        public PaymentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Nuevos campos para trazabilidad
        public Guid? TransactionId { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? GatewayResponse { get; set; }
        public string ExternalTransactionId { get; set; }

        // Campos faltantes para compatibilidad con PaymentGatewayRequest
        public string Currency { get; set; }
        public string PaymentMethodToken { get; set; }
        public string CustomerEmail { get; set; }

        public Pay() { }

        public Pay(
            Guid reservationId,
            decimal amount,
            int methodId,
            string currency,
            string paymentMethodToken,
            string customerEmail,
            string externalTransactionId,
            bool isSuccessful,
            string? gatewayResponse = null,
            Guid? transactionId = null)
        {
            Id = Guid.NewGuid();
            ReservationId = reservationId;
            Amount = amount;
            PaymentMethodId = methodId;
            Currency = currency;
            PaymentMethodToken = paymentMethodToken;
            CustomerEmail = customerEmail;
            ExternalTransactionId = externalTransactionId;
            TransactionId = transactionId;
            CreatedAt = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;
            IsSuccessful = isSuccessful;
            GatewayResponse = gatewayResponse;
            PaymentStatusId = isSuccessful ? 2 : 3;

            if (isSuccessful)
                CompletedAt = DateTime.UtcNow;
        }

        // Métodos de estado
        public void MarkAsCompleted(string? gatewayResponse = null)
        {
            PaymentStatusId = 2;
            CompletedAt = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;
            IsSuccessful = true;
            GatewayResponse = gatewayResponse;
        }

        public void MarkAsFailed(string? gatewayResponse = null)
        {
            PaymentStatusId = 3;
            LastUpdated = DateTime.UtcNow;
            IsSuccessful = false;
            GatewayResponse = gatewayResponse;
        }

        public void Refund(string? gatewayResponse = null)
        {
            PaymentStatusId = 4;
            LastUpdated = DateTime.UtcNow;
            IsSuccessful = false;
            GatewayResponse = gatewayResponse;
        }

        public void AttachTransaction(Guid transactionId)
        {
            TransactionId = transactionId;
            LastUpdated = DateTime.UtcNow;
        }

        public void UpdateStatus(int statusId, bool success, string? gatewayResponse = null)
        {
            PaymentStatusId = statusId;
            IsSuccessful = success;
            LastUpdated = DateTime.UtcNow;
            GatewayResponse = gatewayResponse;
        }
    }
}
