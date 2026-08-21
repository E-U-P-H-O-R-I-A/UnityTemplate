using System;
using UnityEngine;
using NotificationType = Data.NotificationPublicModel.Type;

namespace Data
{
    [Serializable]
    public class NotificationPrivateScheme : PrivateScheme
    {
        [SchemeId] private NotificationType type;
        
        [SerializeField] private int androidNotificationId;
        [SerializeField] private string iosNotificationId;

        public override string ID => type.ToString();
        public string IosNotificationId => iosNotificationId;
        public int AndroidNotificationId => androidNotificationId;

        public NotificationPrivateScheme(NotificationType type) =>
            this.type = type;

        public void SaveNotificationAndroidID(int notificationId) =>
            androidNotificationId = notificationId;

        public void SaveNotificationIosId(string notificationId) =>
            iosNotificationId = notificationId;
    }
}
