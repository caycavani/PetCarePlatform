namespace PetCare.Payment.Application.Interfaces
{
    using PetCare.Payment.Application.DTOs;

    public interface IPaymentService
    {
        Task<Guid> CreateAsync(CreatePayRequest request);
        Task<PayResponse?> GetByIdAsync(Guid id);
        Task<IEnumerable<PayResponse>> GetByReservationAsync(Guid reservationId);
        Task<bool> MarkAsCompletedAsync(Guid id);
        Task<bool> MarkAsFailedAsync(Guid id);
        Task<bool> RefundAsync(Guid id);
    }

}
