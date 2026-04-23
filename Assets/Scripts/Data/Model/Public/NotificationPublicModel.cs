using System.Linq;
using Data.Scheme.Public;
using Extensions.List;
using UnityEngine;

namespace Data.Model.Public
{
    [CreateAssetMenu(menuName = "Models/Notification")]
    public class NotificationPublicModel : BasePublicModel<NotificationPublicScheme>
    {
        public override NotificationPublicScheme GetScheme(string id) => 
            schemes.Where(scheme => scheme.ID == id).ToList().Random();
    }
}