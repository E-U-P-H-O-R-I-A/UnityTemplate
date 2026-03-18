using Cysharp.Threading.Tasks;

namespace Utility.LoadingCurtain
{
    public interface ILoadingCurtain
    {
        public void Show();

        public void Hide();

        public UniTask Finish();

        public UniTask AnimatePhase(UniTask task, float target);
    }
}