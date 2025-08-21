using PetCare.Notification.Application.Ports;
using PetCare.Notification.Domain.Entities;
using PetCare.Notification.Domain.Interfaces;
using System.Threading.Tasks;

namespace PetCare.Notification.Infrastructure.Senders
{
    public class PushNotificationSender : INotificationSender
    {
        private readonly IPushGateway _pushGateway;

        public PushNotificationSender(IPushGateway pushGateway)
        {
            _pushGateway = pushGateway;
        }

        public async Task<bool> SendAsync(Ntification ntification)
        {
            var title = ntification.Content.Subject;
            var body = ntification.Content.Message;
            var deviceToken = ntification.Recipient.DeviceToken;

            if (string.IsNullOrWhiteSpace(deviceToken))
                return false;

            return await _pushGateway.SendPushAsync(deviceToken, title, body);
        }
    }
}
