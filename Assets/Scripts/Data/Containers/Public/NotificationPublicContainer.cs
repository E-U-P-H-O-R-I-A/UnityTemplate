using System.Collections.Generic;
using System.Linq;
using Extensions;

namespace Data
{
    public class NotificationPublicContainer : PublicContainer.Collection<NotificationPublicRecord>
    {
        protected override NotificationPublicRecord GetRecord(string id)
        {
            List<NotificationPublicRecord> matches = records
                .Where(record => record != null && record.Id == id)
                .ToList();

            return matches.Count == 0 ? null : matches.Random();
        }

        #region Generated

        public enum Id
        {
            General = 0,
        }

        #endregion
    }
}
