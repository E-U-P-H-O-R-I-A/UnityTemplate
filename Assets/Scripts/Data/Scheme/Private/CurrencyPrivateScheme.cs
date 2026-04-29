using System;
using Data.Scheme.Public;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data.Scheme.Private
{
    [Serializable]
    public class CurrencyPrivateScheme : BasePrivateScheme
    {
        [SerializeField] private CurrencyType type;
        [SerializeField] private int value;

        public int Value => value;
        public CurrencyType Type => type;
        public override string ID => type.ToString();

        public CurrencyPrivateScheme(CurrencyType type, int value = 0)
        {
            this.type = type;
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
