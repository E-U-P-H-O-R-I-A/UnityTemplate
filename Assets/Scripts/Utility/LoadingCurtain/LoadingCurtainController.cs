using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility.MVC;

namespace Utility.LoadingCurtain
{
    public class LoadingCurtainController : Controller<LoadingCurtainView>, ILoadingCurtainController
    {
        private float progress;

        public bool IsVisible => View.IsVisible;

        public LoadingCurtainController(LoadingCurtainView view) : base(view) { }

        public override void Dispose() =>
            View.Kill();

        public async UniTask Show(bool animated = true)
        {
            if (View.IsVisible)
                return;

            View.Show();
            View.SetContentAlpha(0f);

            SetProgress(0f);

            if (animated)
                await View.PlayCloseAsync();
            else
                View.SetClosed();

            await View.FadeContentAsync(1f);
        }

        public async UniTask Hide(bool animated = true)
        {
            if (!View.IsVisible)
                return;

            await View.FadeContentAsync(0f);

            if (animated)
                await View.PlayOpenAsync();
            else
                View.SetOpened();

            View.Hide();
        }

        public async UniTask Finish()
        {
            while (progress < 1f)
            {
                SetProgress(Mathf.MoveTowards(progress, 1f, Time.deltaTime * 3f));

                await UniTask.Yield();
            }

            await UniTask.WaitForEndOfFrame();
        }

        public async UniTask AnimatePhase(UniTask task, float target)
        {
            target = Mathf.Clamp01(target);

            while (!task.Status.IsCompleted())
            {
                SetProgress(Mathf.MoveTowards(progress, target, Time.deltaTime));

                await UniTask.Yield();
            }

            SetProgress(target);

            await task;
        }

        private void SetProgress(float value)
        {
            progress = value;

            View.SetProgress(value);
        }
    }
}
