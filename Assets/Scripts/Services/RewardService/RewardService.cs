using System.Collections.Generic;
using Data.Scheme.Public;
using Services.CurrencyService;

namespace Services.RewardService
{
    public class RewardService : IRewardService
    {
        private readonly ICurrencyService currencyService;

        public RewardService(ICurrencyService currencyService)
        {
            this.currencyService = currencyService;
        }

        public void GetReward(List<RewardConfig> rewards)
        {
            foreach (var reward in rewards)
            {
                GetReward(reward);
            }
        }

        public void GetReward(RewardConfig reward)
        {
            switch (reward.type)
            {
                case TypeReward.Currency:
                    GiveOutCurrency(reward.currencyType, reward.amount);
                    break;
            }
        }

        private void GiveOutCurrency(CurrencyType currencyType, int amount) => 
            currencyService.IncreaseCurrency(currencyType, amount);
    }
}