using System;
using Data.Scheme.Public;
using Services.CurrencyService;
using Sirenix.OdinInspector;

namespace Services.RewardService
{
    [Serializable]
    public class RewardConfig
    {
        [HorizontalGroup("Header")] 
        public TypeReward type;

        [HorizontalGroup("Info")] 
        [ShowIf("@type == TypeReward.Currency")]
        public CurrencyTransaction currencyTransaction;
    }
}