using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    public abstract class PrivateModel : IPrivateModel
    {
        public abstract string ExportToJson();

        public abstract void ImportFromJson(string json);

        public abstract class Single<TScheme> : PrivateModel where TScheme : PrivateScheme
        {
            private TScheme scheme;

            public TScheme GetScheme() =>
                scheme ??= CreateScheme();

            public override string ExportToJson()
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

            public override void ImportFromJson(string json)
            {
                if (string.IsNullOrWhiteSpace(json))
                    return;

                var dump = JsonUtility.FromJson<SchemesDump>(json);
                if (dump?.items == null || dump.items.Count == 0)
                    return;

                var record = dump.items[0];
                if (string.IsNullOrEmpty(record.payload))
                    return;

                scheme ??= CreateScheme();
                if (scheme == null)
                    return;

                JsonUtility.FromJsonOverwrite(record.payload, scheme);
            }

            protected abstract TScheme CreateScheme();
        }

        public abstract class Collection<TScheme> : PrivateModel where TScheme : PrivateScheme
        {
            private readonly List<TScheme> schemes = new();

            public override string ExportToJson()
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

            public override void ImportFromJson(string json)
            {
                if (string.IsNullOrWhiteSpace(json))
                    return;

                var dump = JsonUtility.FromJson<SchemesDump>(json);
                if (dump?.items == null)
                    return;

                var map = schemes.ToDictionary(scheme => scheme.ID, scheme => scheme);

                foreach (var record in dump.items)
                {
                    if (string.IsNullOrEmpty(record.payload))
                        continue;

                    if (!map.TryGetValue(record.id, out var scheme))
                    {
                        scheme = CreateSchemeById(record.id);
                        if (scheme == null)
                            continue;

                        schemes.Add(scheme);
                        map[record.id] = scheme;
                    }

                    JsonUtility.FromJsonOverwrite(record.payload, scheme);
                }
            }

            public TScheme GetScheme(string id)
            {
                if (TryFindScheme(id, out var scheme))
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

                return schemes.RemoveAll(scheme => scheme.ID == id) > 0;
            }

            protected abstract TScheme CreateSchemeById(string id);

            protected bool TryFindScheme(string id, out TScheme scheme)
            {
                scheme = schemes.FirstOrDefault(item => item.ID == id);
                return scheme != null;
            }
        }
    }
}
