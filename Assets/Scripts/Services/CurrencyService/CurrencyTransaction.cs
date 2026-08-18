using System;
using Data;
using UnityEngine.Serialization;
using CurrencyType = Data.CurrencyPublicModel.Type;

namespace Services.CurrencyService
{
    [Serializable]
    public struct CurrencyTransaction
    {
        public CurrencyType type;
        public int amount;
    }
}
