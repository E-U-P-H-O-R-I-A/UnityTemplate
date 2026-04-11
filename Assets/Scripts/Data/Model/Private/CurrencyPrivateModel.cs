using Data.Scheme.Private;

namespace Data.Model.Private
{
    public class CurrencyPrivateModel : BasePrivateModel<CurrencyPrivateScheme>
    {
        protected override CurrencyPrivateScheme CreateSchemeById(string id)
        {
            return int.TryParse(id, out var parsedId)
                ? new CurrencyPrivateScheme(parsedId)
                : null;
        }
    }
}
