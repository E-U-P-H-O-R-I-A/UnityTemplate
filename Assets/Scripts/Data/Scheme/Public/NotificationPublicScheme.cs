using System;
using UnityEngine;

namespace Data.Scheme.Public
{
    public enum NotificationType
    {
        General = 0,
    }
    
    [Serializable]
    public class NotificationPublicScheme : BasePublicScheme
    {
        [SerializeField] private NotificationType type;
        [Space]
        [SerializeField] private float fireAfterSeconds;
        [Space]
        [SerializeField] private string title = string.Empty;
        [SerializeField] private string message = string.Empty;
        [Space]
        [SerializeField] private string smallIcon = string.Empty;
        [SerializeField] private string largeIcon = string.Empty;
        [Space]
        [SerializeField] private Color style = new(0.173f, 0.612f, 0.302f, 1f);
        
        public override string ID => type.ToString();
        
        public Color Style => style;
        public NotificationType Type => type;

        public string Title => title;
        public string Message => message;
        
        public string SmallIcon => smallIcon;
        public string LargeIcon => largeIcon;
        
        public float FireAfterSeconds
        {
            get => fireAfterSeconds;
            set => fireAfterSeconds = value;
        }
    }
}