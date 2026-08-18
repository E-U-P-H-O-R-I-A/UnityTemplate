using System;
using CurrencyType = Data.CurrencyPublicModel.Type;

namespace Data
{
    public class CurrencyPrivateModel : PrivateModel.Collection<CurrencyPrivateScheme>
    {
        protected override CurrencyPrivateScheme CreateSchemeById(string id)
        {
            return Enum.TryParse(id, out CurrencyType type) 
                ? new CurrencyPrivateScheme(type) 
                : null;
        }
    }
}
