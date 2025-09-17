using Cysharp.Threading.Tasks;

namespace Utility.StateMachine
{
    public interface IStateMachine
    {
        void RegisterState<TState>(TState state) where TState : IState;
        UniTask Enter<TState>() where TState : class, IState;
    }
}