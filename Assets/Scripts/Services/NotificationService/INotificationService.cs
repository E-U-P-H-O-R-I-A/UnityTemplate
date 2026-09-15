
using Data;
using NotificationId = Data.NotificationPublicContainer.Id;

namespace Services.NotificationService
{
    public interface INotificationService
    {
        void Initialize();
        
        void SendNotification(NotificationId type);
        
        void CancelNotification(NotificationId type);
    }
}
