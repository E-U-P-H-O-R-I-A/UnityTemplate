using System;
using NotificationId = Data.NotificationPublicContainer.Id;

namespace Data
{
    public class NotificationPrivateContainer : PrivateContainer.Collection<NotificationPrivateRecord>
    {
        public bool TryGetRecord(string id, out NotificationPrivateRecord record) =>
            TryFindRecord(id, out record);

        protected override NotificationPrivateRecord CreateRecordById(string id)
        {
            return Enum.TryParse(id, out NotificationId type) 
                ? new NotificationPrivateRecord(type) 
                : null;
        }
    }
}
