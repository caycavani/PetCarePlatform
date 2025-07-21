using PetCare.Payment.Application.DTOs;
using PetCare.Payment.Domain.Entities;
using PetCare.Payment.Domain.Interfaces;

namespace PetCare.Payment.Application.UseCases
{
    public class ProcessPaymentUseCase
    {
        private readonly IPaymentRepository _repository;

        public ProcessPaymentUseCase(IPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> ExecuteAsync(CreatePaymentDto dto)
        {
            var payment = new Pay(
                reservationId: dto.ReservationId,
                amount: dto.Amount,
                method: dto.Method
            );

            await _repository.AddAsync(payment);

            return payment.Id;
        }
    }
}
