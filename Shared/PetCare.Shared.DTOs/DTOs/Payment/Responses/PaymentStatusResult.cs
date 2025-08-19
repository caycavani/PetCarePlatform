namespace PetCareShared.DTOs.DTOs.Payment.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PaymentStatusResult
    {
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public DateTime LastUpdated { get; set; }
    }


}
