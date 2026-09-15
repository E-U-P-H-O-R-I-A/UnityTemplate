using System;
using Data;
using Services.LogService;
using Services.PrivateContainerProvider;
using Services.PublicContainerProvider;
#if UNITY_ANDROID
using Cysharp.Threading.Tasks;
using Unity.Notifications.Android;
#endif
using UnityEngine;
using VContainer;
using NotificationId = Data.NotificationPublicContainer.Id;

namespace Services.NotificationService
{
    public class NotificationAndroidService : INotificationService
    {
        private const string CHANNEL_ID = "notification_channel";

        [Inject] private IPrivateContainerProvider privateContainerProvider;
        [Inject] private IPublicContainerProvider publicContainerProvider;
        [Inject] private ILogService logService;

        private NotificationPrivateContainer privateContainer;
        private NotificationPublicContainer publicContainer;

        public void Initialize()
        {
#if UNITY_ANDROID
            publicContainer = publicContainerProvider.GetContainer<NotificationPublicContainer>();
            privateContainer = privateContainerProvider.GetContainer<NotificationPrivateContainer>();

            AndroidNotificationChannel channel = CreateChannel();
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            RequestPermissionAsync().Forget();
#endif
        }

        public void SendNotification(NotificationId type)
        {
#if UNITY_ANDROID
            if (AndroidNotificationCenter.UserPermissionToPost != PermissionStatus.Allowed)
            {
                logService.LogWarning(
                    $"Notification '{type}' was not scheduled because notification permission is not granted.",
                    LogCategory.Service);
                return;
            }

            NotificationPublicRecord publicRecord = publicContainer?.GetRecord(type);
            NotificationPrivateRecord privateRecord = privateContainer?.GetRecord(type.ToString());

            if (publicRecord == null || privateRecord == null)
            {
                logService.LogWarning(
                    $"Notification '{type}' was not scheduled because its record is missing.",
                    LogCategory.Service);
                return;
            }
            
            AndroidNotification  androidNotification = CreateNotification(publicRecord);
            int id = AndroidNotificationCenter.SendNotification(androidNotification, CHANNEL_ID);
            
            logService.Log($"Send notification id: {id}, {publicRecord.Id}, title: {publicRecord.Title}, " +
                           $"message: {publicRecord.Message}; {publicRecord.FireAfterSeconds} seconds to shoot", LogCategory.Service);

            privateRecord.SetAndroidNotificationId(id);
            
            privateContainerProvider.SaveContainer<NotificationPrivateContainer>().Forget();
#endif
        }

        public void CancelNotification(NotificationId type)
        {
#if UNITY_ANDROID
            if (privateContainer == null || !privateContainer.TryGetRecord(type.ToString(), out NotificationPrivateRecord privateRecord))
                return;

            AndroidNotificationCenter.CancelNotification(privateRecord.AndroidNotificationId);

            logService.Log($"Cancelled notification id: {privateRecord.AndroidNotificationId}, {type}", LogCategory.Service);
            
            privateContainer.DeleteRecordById(privateRecord.Id);
            privateContainerProvider.SaveContainer<NotificationPrivateContainer>().Forget();
#endif
        }
        
#if UNITY_ANDROID
        private async UniTask RequestPermissionAsync()
        {
            var request = new PermissionRequest();

            while (request.Status == PermissionStatus.RequestPending)
                await UniTask.Yield();

            if (request.Status != PermissionStatus.Allowed)
            {
                logService.LogWarning(
                    $"Notification permission was not granted. Status: {request.Status}.",
                    LogCategory.Service);
            }
        }

        private AndroidNotificationChannel CreateChannel() => new()
        {
            Id = CHANNEL_ID,
            Name = "Notifications Channel",
            Importance = Importance.High,
            Description = "Generic notifications",
        };

        private AndroidNotification CreateNotification(NotificationPublicRecord setting) => new()
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
