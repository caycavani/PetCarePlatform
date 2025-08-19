namespace PetCare.Payment.Domain.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PetCare.Payment.Domain.Entities;

    public interface IPaymentMethodRepository
    {
        Task<IEnumerable<PaymentMethod>> GetAllAsync();

        Task<PaymentMethod?> GetByIdAsync(int id);
    }
}
