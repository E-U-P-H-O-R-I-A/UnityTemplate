using Data;
using Services.LogService;
using Services.PrivateContainerProvider;
using Services.PublicContainerProvider;
using VContainer;
using NotificationId = Data.NotificationPublicContainer.Id;
#if UNITY_IOS
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Unity.Notifications.iOS;
#endif

namespace Services.NotificationService
{
    public class NotificationIOSService : INotificationService
    {
        [Inject] private IPrivateContainerProvider privateContainerProvider;
        [Inject] private IPublicContainerProvider publicContainerProvider;
        [Inject] private ILogService logService;

        private NotificationPrivateContainer privateContainer;
        private NotificationPublicContainer publicContainer;

        public void Initialize()
        {
#if UNITY_IOS
            publicContainer = publicContainerProvider.GetContainer<NotificationPublicContainer>();
            privateContainer = privateContainerProvider.GetContainer<NotificationPrivateContainer>();

            _ = new AuthorizationRequest(
                AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound,
                false
            );
#endif
        }

        public void SendNotification(NotificationId type)
        {
#if UNITY_IOS

            NotificationPublicRecord publicRecord = publicContainer?.GetRecord(type);
            NotificationPrivateRecord privateRecord = privateContainer?.GetRecord(type.ToString());

            if (publicRecord == null || privateRecord == null)
            {
                logService.LogWarning(
                    $"Notification '{type}' was not scheduled because its record is missing.",
                    LogCategory.Service);
                return;
            }
            
            string id = CreateIdentifier(publicRecord);
            iOSNotification notification = CreateNotification(publicRecord, id);
            iOSNotificationCenter.ScheduleNotification(notification);

            logService.Log($"Send notification id: {id}, {publicRecord.Id}, title: {publicRecord.Title}, " +
                           $"message: {publicRecord.Message}; {publicRecord.FireAfterSeconds} seconds to shoot", LogCategory.Service);

            privateRecord.SetIosNotificationId(id);
            privateContainerProvider.SaveContainer<NotificationPrivateContainer>().Forget();
#endif
        }

        public void CancelNotification(NotificationId type)
        {
#if UNITY_IOS
            if (privateContainer == null || !privateContainer.TryGetRecord(type.ToString(), out NotificationPrivateRecord privateRecord))
                return;

            iOSNotificationCenter.RemoveScheduledNotification(privateRecord.IosNotificationId);
            iOSNotificationCenter.RemoveDeliveredNotification(privateRecord.IosNotificationId);

            logService.Log($"Cancelled notification id: {privateRecord.IosNotificationId}, {type}", LogCategory.Service);

            privateContainer.DeleteRecordById(privateRecord.Id);
            privateContainerProvider.SaveContainer<NotificationPrivateContainer>().Forget();
#endif
        }

#if UNITY_IOS
        private string CreateIdentifier(NotificationPublicRecord settings) =>
            $"notification_{settings.Id}_{DateTime.UtcNow.Ticks}";

        private static iOSNotification CreateNotification(NotificationPublicRecord settings, string id) => new()
        {
            Identifier = id,
            Title = settings.Title,
            Body = settings.Message,
            ThreadIdentifier = settings.Id,
            Trigger = new iOSNotificationTimeIntervalTrigger
            {
                TimeInterval = TimeSpan.FromSeconds(Mathf.Max(1, settings.FireAfterSeconds)),
                Repeats = false
            }
        };
#endif
    }
}
