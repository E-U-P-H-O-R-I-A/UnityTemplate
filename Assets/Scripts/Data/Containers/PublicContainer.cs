using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    public abstract class PublicContainer : ScriptableObject, IPublicContainer
    {
        public abstract class Single<TRecord> : PublicContainer where TRecord : PublicRecord, new()
        {
            [SerializeField] protected TRecord record = new();

            public TRecord GetRecord() =>
                record ??= new TRecord();
        }

        public abstract class Collection<TRecord> : PublicContainer where TRecord : PublicRecord
        {
            [SerializeField] protected List<TRecord> records = new();

            public IReadOnlyList<TRecord> Records => records;

            public TRecord GetRecord<TId>(TId id) where TId : struct, Enum =>
                GetRecord(id.ToString());

            protected virtual TRecord GetRecord(string id) =>
                records.FirstOrDefault(record => record != null && record.Id == id);
        }
    }
}
