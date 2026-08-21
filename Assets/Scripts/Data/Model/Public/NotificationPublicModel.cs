using System.Collections.Generic;
using System.Linq;
using Extensions;

namespace Data
{
    public class NotificationPublicModel : PublicModel.Collection<NotificationPublicScheme>
    {
        protected override NotificationPublicScheme GetScheme(string id)
        {
            List<NotificationPublicScheme> matches = schemes
                .Where(scheme => scheme != null && scheme.ID == id)
                .ToList();

            return matches.Count == 0 ? null : matches.Random();
        }

        #region Generated

        public enum Type
        {
            General = 0,
        }

        #endregion
    }
}
