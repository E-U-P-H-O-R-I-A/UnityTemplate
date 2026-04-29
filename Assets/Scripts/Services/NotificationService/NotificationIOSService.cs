using Data.Model.Private;
using Data.Model.Public;
using Data.Scheme.Public;
using Services.LogService;
using Services.PrivateModelProvider;
using Services.PublicModelProvider;
using VContainer;
#if UNITY_IOS
using System;
using UnityEngine;
using Data.Scheme.Private;
using Unity.Notifications.iOS;
#endif

namespace Services.NotificationService
{
    public class NotificationIOSService : INotificationService
    {
        [Inject] private IPrivateModelProvider privateModelProvider;
        [Inject] private IPublicModelProvider publicModelProvider;
        [Inject] private ILogService logService;

        private NotificationPrivateModel collectionPrivateModel;
        private NotificationPublicModel publicModel;

        public void Initialize()
        {
#if UNITY_IOS
            publicModel = publicModelProvider.GetModel<NotificationPublicModel>();
            privateModel = privateModelProvider.GetModel<NotificationPrivateModel>();

            _ = new AuthorizationRequest(
                AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound,
                false
            );
#endif
        }

        public void SendNotification(NotificationType type)
        {
#if UNITY_IOS

            NotificationPublicScheme publicScheme = publicModel.GetScheme(type.ToString());
            NotificationPrivateScheme privateScheme = privateModel.GetScheme(type.ToString());
            
            string id = CreateIdentifier(publicScheme);
            iOSNotification notification = CreateNotification(publicScheme, id);
            iOSNotificationCenter.ScheduleNotification(notification);

            logService.Log($"Send notification id: {id}, {publicScheme.Type}, title: {publicScheme.Title}, " +
                           $"message: {publicScheme.Message}; {publicScheme.FireAfterSeconds} seconds to shoot", LogCategory.Service);

            privateScheme.SaveNotificationIosId(id);
            privateModelProvider.SaveModel<NotificationPrivateModel>();
#endif
        }

        public void CancelNotification(NotificationType type)
        {
#if UNITY_IOS
            if (!privateModel.IsHaveScheme(type.ToString()))
                return;
            
            NotificationPrivateScheme privateScheme = privateModel.GetScheme(type.ToString());

            iOSNotificationCenter.RemoveScheduledNotification(privateScheme.IosNotificationId);
            iOSNotificationCenter.RemoveDeliveredNotification(privateScheme.IosNotificationId);

            logService.Log($"Cancelled notification id: {privateScheme.IosNotificationId}, {type}", LogCategory.Service);

            privateModel.DeleteSchemeById(privateScheme.ID);
            privateModelProvider.SaveModel<NotificationPrivateModel>();
#endif
        }

#if UNITY_IOS
        private string CreateIdentifier(NotificationPublicScheme settings) =>
            $"notification_{settings.Type}_{DateTime.UtcNow.Ticks}";

        private static iOSNotification CreateNotification(NotificationPublicScheme settings, string id) => new()
        {
            Identifier = id,
            Title = settings.Title,
            Body = settings.Message,
            ThreadIdentifier = settings.Type.ToString(),
            Trigger = new iOSNotificationTimeIntervalTrigger
            {
                TimeInterval = TimeSpan.FromSeconds(Mathf.Max(1, settings.FireAfterSeconds)),
                Repeats = false
            }
        };
#endif
    }
}
