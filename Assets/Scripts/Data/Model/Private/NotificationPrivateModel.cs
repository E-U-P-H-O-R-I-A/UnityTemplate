using System;
using Data.Scheme.Private;
using Data.Scheme.Public;

namespace Data.Model.Private
{
    public class NotificationPrivateModel : BasePrivateModel<NotificationPrivateScheme>
    {
        protected override NotificationPrivateScheme CreateSchemeById(string id)
        {
            return Enum.TryParse(id, out NotificationType type) 
                ? new NotificationPrivateScheme(type) 
                : null;
        }
    }
}