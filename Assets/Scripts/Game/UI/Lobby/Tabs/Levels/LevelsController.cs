using DG.Tweening;
using Utility.MVC;

namespace Game.UI.Lobby.Tabs.Levels
{
    public class LevelsController : Controller<LevelsView>
    {
        private readonly LevelsModel model;

        private Sequence swapSequence;
        private bool isNeedAnimation;

        public LevelsController(LevelsView view, LevelsModel model) : base(view)
        {
            this.model = model;
        }

        public override void Initialize()
        {
            isNeedAnimation = model.CurrentLevel > 0 && model.IsLevelCompleted;

            model.ConsumeCompletedFlag();

            Fill();

            if (isNeedAnimation)
                PlaySwapAnimation();
        }

        public override void Dispose()
        {
            swapSequence?.Kill();
            swapSequence = null;

            View.Clear();
        }

        private void Fill()
        {
            int levelNumber = model.CurrentLevelNumber;
            int amount = View.AmountShowingLevels;

            if (isNeedAnimation)
            {
                levelNumber--;
                amount++;
            }

            for (int index = 0; index < amount; index++)
            {
                LevelItemView item = View.CreateItem();
                LevelItemState state = index == 0 ? LevelItemState.Open : LevelItemState.Close;

                item.Initialize(state, levelNumber + index);
            }
        }

        private void PlaySwapAnimation()
        {
            View.SetScrollEnabled(false);

            swapSequence?.Kill();
            swapSequence = View.CreateSwapSequence();

            swapSequence.OnComplete(() =>
            {
                View.SetScrollEnabled(true);
                View.ResetScroll();
                View.Items[0].Hide();
            });
        }
    }
}
