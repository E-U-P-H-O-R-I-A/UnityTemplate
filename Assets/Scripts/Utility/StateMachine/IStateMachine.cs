namespace Utility.StateMachine
{
    public interface IStateMachine
    {
        void RegisterState(IExitableState state);
        void Enter<TState>() where TState : class, IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>;
    }
}
