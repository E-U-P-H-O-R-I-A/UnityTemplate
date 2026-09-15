using System;
using CurrencyId = Data.CurrencyPublicContainer.Id;

namespace Data
{
    public class CurrencyPrivateContainer : PrivateContainer.Collection<CurrencyPrivateRecord>
    {
        protected override CurrencyPrivateRecord CreateRecordById(string id)
        {
            return Enum.TryParse(id, out CurrencyId type) 
                ? new CurrencyPrivateRecord(type) 
                : null;
        }
    }
}
