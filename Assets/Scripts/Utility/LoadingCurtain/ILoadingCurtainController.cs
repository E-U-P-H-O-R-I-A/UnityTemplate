using Cysharp.Threading.Tasks;
using Utility.MVC;

namespace Utility.LoadingCurtain
{
    public interface ILoadingCurtainController : IController
    {
        bool IsVisible { get; }

        UniTask Show(bool animated = true);

        UniTask Hide(bool animated = true);

        UniTask Finish();

        UniTask AnimatePhase(UniTask task, float target);
    }
}
