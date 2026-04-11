using System;
using Data.Scheme.Public;
using UnityEngine;

namespace Data.Scheme.Private
{
    [Serializable]
    public class CurrencyPrivateScheme : BasePrivateScheme
    {
        [SerializeField] private int id;
        [SerializeField] private int value;

        public int Value => value;
        public override string ID => id.ToString();
        public CurrencyType Type => (CurrencyType)id;

        public CurrencyPrivateScheme(int id, int value = 0)
        {
            this.id = id;
            this.value = value;
        }

        public bool IsEnoughCurrency(int value)
        {
            return this.value >= value;
        }

        public void IncreaseCurrency(int value)
        {
            this.value += value;
        }

        public void DecreaseCurrency(int value)
        {
            this.value -= value;
        }
    }
}
