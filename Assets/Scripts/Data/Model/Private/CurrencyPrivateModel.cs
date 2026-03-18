using Data.Scheme.Private;

namespace Data.Model.Private
{
    public class CurrencyPrivateModel : BasePrivateModel<CurrencyPrivateScheme>
    {
        protected override CurrencyPrivateScheme CreateSchemeById(int id)
        {
            return new CurrencyPrivateScheme(id);
        }
    }
}