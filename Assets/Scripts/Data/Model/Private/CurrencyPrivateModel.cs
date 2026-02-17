using Data.Scheme.Private;
using Data.Scheme.Public;

namespace Data.Model
{
    public class CurrencyPrivateModel : BasePrivateModel<CurrencyPrivateScheme>
    {
        protected override CurrencyPrivateScheme CreateSchemeById(int id)
        {
            return new CurrencyPrivateScheme((CurrencyType)id);
        }
    }
}