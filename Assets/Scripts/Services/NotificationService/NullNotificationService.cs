using NotificationId = Data.NotificationPublicContainer.Id;

namespace Services.NotificationService
{
    public class NullNotificationService : INotificationService
    {
        public void Initialize() { }

        public void SendNotification(NotificationId type) { }

        public void CancelNotification(NotificationId type) { }
    }
}
