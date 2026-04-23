using System;
using Data.Model.Private;
using Data.Model.Public;
using Data.Scheme.Private;
using Data.Scheme.Public;
using Services.LogService;
using Services.PrivateModelProvider;
using Services.PublicModelProvider;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;
using VContainer;

namespace Services.NotificationService
{
    public class NotificationAndroidService : INotificationService
    {
        private const string CHANNEL_ID = "notification_channel";

        [Inject] private IPrivateModelProvider privateModelProvider;
        [Inject] private IPublicModelProvider publicModelProvider;
        [Inject] private ILogService logService;

        private NotificationPrivateModel privateModel;
        private NotificationPublicModel publicModel;

        public void Initialize()
        {
#if UNITY_ANDROID
            publicModel = publicModelProvider.GetModel<NotificationPublicModel>();
            privateModel = privateModelProvider.GetModel<NotificationPrivateModel>();

            AndroidNotificationChannel channel = CreateChannel();
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
        }

        public void SendNotification(NotificationType type)
        {
#if UNITY_ANDROID
            NotificationPublicScheme publicScheme = publicModel.GetScheme(type.ToString());
            NotificationPrivateScheme privateScheme = privateModel.GetScheme(type.ToString());
            
            AndroidNotification  androidNotification = CreateNotification(publicScheme);
            int id = AndroidNotificationCenter.SendNotification(androidNotification, CHANNEL_ID);
            
            logService.Log($"Send notification id: {id}, {publicScheme.Type}, title: {publicScheme.Title}, " +
                           $"message: {publicScheme.Message}; {publicScheme.FireAfterSeconds} seconds to shoot", LogCategory.Service);

            privateScheme.SaveNotificationAndroidID(id);
            
            privateModelProvider.SaveModel<NotificationPrivateModel>();
#endif
        }

        public void CancelNotification(NotificationType type)
        {
#if UNITY_ANDROID
            if (!privateModel.IsHaveScheme(type.ToString()))
                return;
            
            NotificationPrivateScheme privateScheme = privateModel.GetScheme(type.ToString());
            AndroidNotificationCenter.CancelNotification(privateScheme.AndroidNotificationId);

            logService.Log($"Cancelled notification id: {privateScheme.AndroidNotificationId}, {type}", LogCategory.Service);
            
            privateModel.DeleteSchemeById(privateScheme.ID);
            privateModelProvider.SaveModel<NotificationPrivateModel>();
#endif
        }
        
#if UNITY_ANDROID
        private AndroidNotificationChannel CreateChannel() => new()
        {
            Id = CHANNEL_ID,
            Name = "Notifications Channel",
            Importance = Importance.High,
            Description = "Generic notifications",
        };

        private AndroidNotification CreateNotification(NotificationPublicScheme setting) => new()
        {
            Title = setting.Title,
            Text = setting.Message,
            SmallIcon = setting.SmallIcon,
            LargeIcon = setting.LargeIcon,
            FireTime = DateTime.Now.AddSeconds(setting.FireAfterSeconds),
            Color = setting.Style
        };
#endif
    }
}