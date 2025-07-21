namespace PetCare.Notification.Application.UseCases
{
    using PetCare.Notification.Application.DTOs;
    using PetCare.Notification.Domain.Entities;
    using PetCare.Notification.Domain.Interfaces;

    public class SendNotificationUseCase
    {
        private readonly INotificationRepository _repository;

        public SendNotificationUseCase(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> ExecuteAsync(CreateNotificationDto dto)
        {
            var notification = new RNotification(
                recipientId: dto.RecipientId,
                message: dto.Message
            );

            await _repository.AddAsync(notification);

            return notification.Id;
        }
    }
}
