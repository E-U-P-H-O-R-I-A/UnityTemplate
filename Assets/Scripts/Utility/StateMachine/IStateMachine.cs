namespace Utility.StateMachine
{
    public interface IStateMachine
    {
        void RegisterState<TState>(TState state) where TState : IState;
        void Enter<TState>() where TState : class, IState;
    }
}
