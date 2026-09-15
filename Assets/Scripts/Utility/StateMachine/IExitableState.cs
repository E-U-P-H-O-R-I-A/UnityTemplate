using Cysharp.Threading.Tasks;

namespace Utility.StateMachine
{
    public interface IExitableState
    {
        UniTask Exit();
    }
}
