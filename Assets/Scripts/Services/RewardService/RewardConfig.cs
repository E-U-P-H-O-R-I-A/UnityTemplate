using System;
using Data.Scheme.Public;
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
        public CurrencyType currencyType;
        
        [HorizontalGroup("Info")] 
        [ShowIf(nameof(InNeedAmount))]
        public int amount;
        
        private bool InNeedAmount() => 
            type is TypeReward.Currency;
    }
}