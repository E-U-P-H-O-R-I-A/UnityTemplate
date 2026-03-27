using System.Collections.Generic;

namespace Services.RewardService
{
    public interface IRewardService
    {
        public void GetReward(RewardConfig reward);
        public void GetReward(List<RewardConfig> rewardsConfig);
    }
}