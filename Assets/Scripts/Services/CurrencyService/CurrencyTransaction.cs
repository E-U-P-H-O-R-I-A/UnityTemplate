using System;
using Data.Scheme.Public;
using UnityEngine.Serialization;

namespace Services.CurrencyService
{
    [Serializable]
    public struct CurrencyTransaction
    {
        public CurrencyType type;
        public int amount;
    }
}