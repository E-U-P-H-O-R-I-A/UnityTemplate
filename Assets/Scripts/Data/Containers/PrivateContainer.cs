using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    public abstract class PrivateContainer : IPrivateContainer
    {
        public abstract string ExportToJson();

        public abstract void ImportFromJson(string json);

        public abstract class Single<TRecord> : PrivateContainer where TRecord : PrivateRecord
        {
            private TRecord record;

            public TRecord GetRecord() =>
                record ??= CreateRecord();

            public override string ExportToJson()
            {
                var dump = new RecordDump();
                var currentRecord = GetRecord();

                if (currentRecord != null)
                {
                    dump.items.Add(new RecordEntry
                    {
                        id = currentRecord.Id,
                        payload = JsonUtility.ToJson(currentRecord)
                    });
                }

                return JsonUtility.ToJson(dump);
            }

            public override void ImportFromJson(string json)
            {
                if (string.IsNullOrWhiteSpace(json))
                    return;

                var dump = JsonUtility.FromJson<RecordDump>(json);
                if (dump?.items == null || dump.items.Count == 0)
                    return;

                var entry = dump.items[0];
                if (string.IsNullOrEmpty(entry.payload))
                    return;

                record ??= CreateRecord();
                if (record == null)
                    return;

                JsonUtility.FromJsonOverwrite(entry.payload, record);
            }

            protected abstract TRecord CreateRecord();
        }

        public abstract class Collection<TRecord> : PrivateContainer where TRecord : PrivateRecord
        {
            private readonly List<TRecord> records = new();

            public override string ExportToJson()
            {
                var dump = new RecordDump();

                foreach (var record in records)
                {
                    dump.items.Add(new RecordEntry
                    {
                        id = record.Id,
                        payload = JsonUtility.ToJson(record)
                    });
                }

                return JsonUtility.ToJson(dump);
            }

            public override void ImportFromJson(string json)
            {
                if (string.IsNullOrWhiteSpace(json))
                    return;

                var dump = JsonUtility.FromJson<RecordDump>(json);
                if (dump?.items == null)
                    return;

                var map = new Dictionary<string, TRecord>();

                foreach (var existing in records)
                {
                    if (existing != null && !string.IsNullOrEmpty(existing.Id))
                        map[existing.Id] = existing;
                }

                foreach (var entry in dump.items)
                {
                    if (entry == null || string.IsNullOrEmpty(entry.id) || string.IsNullOrEmpty(entry.payload))
                        continue;

                    if (!map.TryGetValue(entry.id, out var record))
                    {
                        record = CreateRecordById(entry.id);

                        if (record == null)
                            continue;

                        records.Add(record);
                        map[entry.id] = record;
                    }

                    JsonUtility.FromJsonOverwrite(entry.payload, record);

                    if (record.Id == entry.id)
                        continue;

                    records.Remove(record);
                    map.Remove(entry.id);
                }
            }

            public TRecord GetRecord(string id)
            {
                if (TryFindRecord(id, out var record))
                    return record;

                record = CreateRecordById(id);

                if (record != null)
                    records.Add(record);

                return record;
            }

            public bool DeleteRecordById(string id)
            {
                if (string.IsNullOrEmpty(id))
                    return false;

                return records.RemoveAll(record => record.Id == id) > 0;
            }

            protected abstract TRecord CreateRecordById(string id);

            protected bool TryFindRecord(string id, out TRecord record)
            {
                record = records.FirstOrDefault(item => item.Id == id);
                return record != null;
            }
        }
    }
}
