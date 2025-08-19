namespace PetCare.Payment.Domain.Interfaces
{
    using System.Threading.Tasks;
    using PetCare.Payment.Domain.Entities;

    public interface IPaymentStatusRepository
    {
        Task<PaymentStatus?> GetByIdAsync(int id);
    }

}
