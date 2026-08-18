
using Data;
using NotificationType = Data.NotificationPublicModel.Type;

namespace Services.NotificationService
{
    public interface INotificationService
    {
        void Initialize();
        
        void SendNotification(NotificationType type);
        
        void CancelNotification(NotificationType type);
    }
}
