using Data.Scheme;
using Data.SaveLoad;
using UnityEngine;

namespace Data.Model
{
    public abstract class BaseSinglePrivateModel<TScheme> : IPrivateModel where TScheme : BasePrivateScheme
    {
        private TScheme scheme;
        
        public TScheme GetScheme() => 
            scheme ??= CreateScheme();
        
        public string ExportToJson()
        {
            var dump = new SchemesDump();
            var currentScheme = GetScheme();

            if (currentScheme != null)
            {
                dump.items.Add(new SchemeRecord
                {
                    id = currentScheme.ID,
                    payload = JsonUtility.ToJson(currentScheme)
                });
            }

            return JsonUtility.ToJson(dump);
        }

        public void ImportFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return;

            var dump = JsonUtility.FromJson<SchemesDump>(json);
            if (dump?.items == null || dump.items.Count == 0)
                return;

            var rec = dump.items[0];
            if (string.IsNullOrEmpty(rec.payload))
                return;

            scheme ??= CreateScheme();
            if (scheme == null)
                return;

            JsonUtility.FromJsonOverwrite(rec.payload, scheme);
        }

        protected abstract TScheme CreateScheme();
    }
}
