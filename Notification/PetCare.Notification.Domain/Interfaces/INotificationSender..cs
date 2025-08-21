using PetCare.Notification.Domain.Entities;
using System.Threading.Tasks;

namespace PetCare.Notification.Domain.Interfaces
{
    public interface INotificationSender
    {
        Task<bool> SendAsync(Ntification notification);
    }

}
