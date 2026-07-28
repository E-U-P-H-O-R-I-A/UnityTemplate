using System.Linq;
using Extensions.List;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Models/Notification")]
    public class NotificationPublicModel : PublicModel<NotificationPublicScheme>
    {
        public override NotificationPublicScheme GetScheme(string id) => 
            schemes.Where(scheme => scheme.ID == id).ToList().Random();
    }
}