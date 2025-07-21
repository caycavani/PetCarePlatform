namespace PetCare.Payment.Domain.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PetCare.Payment.Domain.Entities;

    public interface IPaymentRepository
    {
        Task AddAsync(Pay payment);

        Task<Pay?> GetByIdAsync(Guid id);

        Task<IEnumerable<Pay>> GetByReservationAsync(Guid reservationId);

        Task<IEnumerable<Pay>> GetAllAsync();

        Task DeleteAsync(Guid id);
    }
}
