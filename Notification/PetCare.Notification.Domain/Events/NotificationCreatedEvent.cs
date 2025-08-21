using PetCare.Notification.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.Notification.Domain.Events
{
    public class NotificationCreatedEvent
    {
        public Ntification Notification { get; }

        public NotificationCreatedEvent(Ntification notification)
        {
            Notification = notification;
        }
    }

}
