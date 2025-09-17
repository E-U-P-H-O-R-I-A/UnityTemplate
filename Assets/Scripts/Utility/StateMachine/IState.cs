using Cysharp.Threading.Tasks;

namespace Utility.StateMachine
{
    public interface IState
    {
        UniTask Enter();
        
        UniTask Exit();
    }
}