
using Data.Scheme.Public;

namespace Services.NotificationService
{
    public interface INotificationService
    {
        void Initialize();
        
        void SendNotification(NotificationType type);
        
        void CancelNotification(NotificationType type);
    }
}