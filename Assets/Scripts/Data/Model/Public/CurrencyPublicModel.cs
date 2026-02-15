using System.Collections.Generic;
using Data.Scheme.Public;
using UnityEngine;

namespace Data.Model.Public
{
    [CreateAssetMenu(menuName = "Schemes/Currency")]
    public class CurrencyPublicModel : BasePublicModel<CurrencyPublicScheme>
    {
        [SerializeField] private List<CurrencyPublicScheme> schemes;

        protected override List<CurrencyPublicScheme> Schemes => schemes;
    }
}