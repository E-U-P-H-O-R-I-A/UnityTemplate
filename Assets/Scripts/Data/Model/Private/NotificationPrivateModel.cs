using System;

namespace Data
{
    public class NotificationPrivateModel : PrivateModel.Collection<NotificationPrivateScheme>
    {
        public bool TryGetScheme(string id, out NotificationPrivateScheme scheme) =>
            TryFindScheme(id, out scheme);

        protected override NotificationPrivateScheme CreateSchemeById(string id)
        {
            return Enum.TryParse(id, out NotificationType type) 
                ? new NotificationPrivateScheme(type) 
                : null;
        }
    }
}
