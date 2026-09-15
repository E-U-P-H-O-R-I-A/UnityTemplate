using System;
using CurrencyId = Data.CurrencyPublicContainer.Id;

namespace Services.CurrencyService
{
    [Serializable]
    public struct CurrencyTransaction
    {
        public CurrencyId type;
        public int amount;
    }
}
