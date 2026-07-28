using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class NotificationPrivateScheme : PrivateScheme
    {
        [SerializeField] private NotificationType type;
        [SerializeField] private int androidNotificationId;
        [SerializeField] private string iosNotificationId;
        
        public override string ID => type.ToString();
        public string IosNotificationId => iosNotificationId;
        public int AndroidNotificationId => androidNotificationId;
        
        public NotificationPrivateScheme(NotificationType type) => 
            this.type = type;

        public void SaveNotificationAndroidID(int id) => 
            androidNotificationId = id;

        public void SaveNotificationIosId(string id) => 
            iosNotificationId = id;
    }
}