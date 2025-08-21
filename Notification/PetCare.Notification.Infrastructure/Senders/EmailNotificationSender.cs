using PetCare.Notification.Domain.Entities;
using PetCare.Notification.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.Notification.Infrastructure.Senders
{
    public class EmailNotificationSender : INotificationSender
    {
        public async Task<bool> SendAsync(Ntification notification)
        {
            // Lógica para enviar email usando SMTP, SendGrid, etc.
            return true;
        }
    }

}
