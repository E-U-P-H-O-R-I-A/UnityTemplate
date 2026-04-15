using System;
using Data.Scheme.Private;
using Data.Scheme.Public;

namespace Data.Model.Private
{
    public class CurrencyPrivateModel : BasePrivateModel<CurrencyPrivateScheme>
    {
        protected override CurrencyPrivateScheme CreateSchemeById(string id)
        {
            return Enum.TryParse(id, out CurrencyType type) 
                ? new CurrencyPrivateScheme(type) 
                : null;
        }
    }
}
