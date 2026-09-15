using System;
using UnityEngine;
using NotificationId = Data.NotificationPublicContainer.Id;

namespace Data
{
    [Serializable]
    public class NotificationPrivateRecord : PrivateRecord
    {
        [RecordId] private NotificationId id;
        
        [SerializeField] private int androidNotificationId;
        [SerializeField] private string iosNotificationId;

        public override string Id => id.ToString();
        public string IosNotificationId => iosNotificationId;
        public int AndroidNotificationId => androidNotificationId;

        public NotificationPrivateRecord(NotificationId id) =>
            this.id = id;

        public void SetAndroidNotificationId(int notificationId) =>
            androidNotificationId = notificationId;

        public void SetIosNotificationId(string notificationId) =>
            iosNotificationId = notificationId;
    }
}
