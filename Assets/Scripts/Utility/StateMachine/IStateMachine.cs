namespace Utility.StateMachine
{
    public interface IStateMachine
    {
        void RegisterState(IState state);
        void Enter<TState>() where TState : class, IState;
    }
}
