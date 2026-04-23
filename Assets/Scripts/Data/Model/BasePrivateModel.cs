using System.Collections.Generic;
using System.Linq;
using Data.SaveLoad;
using Data.Scheme;
using UnityEngine;

namespace Data.Model
{
    public abstract class BasePrivateModel<TScheme> : IPrivateModel where TScheme : BasePrivateScheme
    {
        private readonly List<TScheme> schemes = new();
        
        public bool IsHaveScheme(string id)
        {
            return schemes.Any(s => s.ID == id);
        }

        public TScheme GetScheme(string id)
        {
            var scheme = schemes.FirstOrDefault(s => s.ID == id);

            if (scheme != null)
                return scheme;
            
            scheme = CreateSchemeById(id);

            if (scheme != null)
                schemes.Add(scheme);

            return scheme;
        }

        public bool DeleteSchemeById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            return schemes.RemoveAll(s => s.ID == id) > 0;
        }
        
        public string ExportToJson()
        {
            var dump = new SchemesDump();

            foreach (var scheme in schemes)
            {
                dump.items.Add(new SchemeRecord
                {
                    id = scheme.ID,
                    payload = JsonUtility.ToJson(scheme)
                });
            }

            return JsonUtility.ToJson(dump);
        }
        
        public void ImportFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return;

            var dump = JsonUtility.FromJson<SchemesDump>(json);
            if (dump?.items == null)
                return;

            var map = schemes.ToDictionary(s => s.ID, s => s);

            foreach (var rec in dump.items)
            {
                if (string.IsNullOrEmpty(rec.payload))
                    continue;

                if (!map.TryGetValue(rec.id, out var scheme))
                {
                    scheme = CreateSchemeById(rec.id);
                    if (scheme == null)
                        continue;

                    schemes.Add(scheme);
                    map[rec.id] = scheme;
                }
                
                JsonUtility.FromJsonOverwrite(rec.payload, scheme);
            }
        }
        
        protected abstract TScheme CreateSchemeById(string id);
    }
}
