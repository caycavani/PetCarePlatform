using PetCare.Notification.Application.Ports;
using PetCare.Notification.Domain.Entities;
using PetCare.Notification.Domain.Interfaces;
using System.Threading.Tasks;

namespace PetCare.Notification.Infrastructure.Senders
{
    /// <summary>
    /// Adaptador secundario para el envío de notificaciones SMS.
    /// </summary>
    public class SmsNotificationSender : INotificationSender
    {
        private readonly ISmsGateway _smsGateway;

        public SmsNotificationSender(ISmsGateway smsGateway)
        {
            _smsGateway = smsGateway;
        }

        public async Task<bool> SendAsync(Ntification ntification)
        {
            var subject = ntification.Content.Subject;
            var message = ntification.Content.Message;
            var phoneNumber = ntification.Recipient.PhoneNumber;

            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            var fullMessage = $"{subject}: {message}";
            return await _smsGateway.SendSmsAsync(phoneNumber, fullMessage);
        }
    }
}
