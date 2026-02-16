using Data.Scheme.Public;
using UnityEngine;

namespace Data.Model.Public
{
    [CreateAssetMenu(menuName = "Schemes/Currency")]
    public class CurrencyPublicModel : BasePublicModel<CurrencyPublicScheme>
    {
        public const string ID = "Currency";
        public override string Id => ID;
    }
}