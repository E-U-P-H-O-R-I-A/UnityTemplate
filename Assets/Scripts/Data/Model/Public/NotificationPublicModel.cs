using System.Linq;
using Extensions;

namespace Data
{
    public class NotificationPublicModel : PublicModel.Collection<NotificationPublicScheme>
    {
        protected override NotificationPublicScheme GetScheme(string id) =>
            schemes.Where(scheme => scheme != null && scheme.ID == id).ToList().Random();

        #region Generated

        public enum Type
        {
            General = 0,
        }

        #endregion
    }
}
