using System;
using UnityEngine;
using CurrencyId = Data.CurrencyPublicContainer.Id;

namespace Data
{
    [Serializable]
    public class CurrencyPrivateRecord : PrivateRecord
    {
        [RecordId] private CurrencyId id;
        
        [SerializeField] private int value;

        public int Value => value;
        public override string Id => id.ToString();

        public CurrencyPrivateRecord(CurrencyId id, int value = 0)
        {
            this.id = id;
            this.value = value;
        }

        public bool IsEnough(int amount)
        {
            return value >= amount;
        }

        public void Increase(int amount)
        {
            value += amount;
        }

        public void Decrease(int amount)
        {
            value -= amount;
        }
    }
}
