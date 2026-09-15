using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Services.LogService;

namespace Utility.StateMachine
{
    public abstract class StateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IExitableState> registeredStates = new();
        private readonly Queue<Transition> pendingTransitions = new();
        private readonly ILogService logService;

        private IExitableState currentState;
        private bool isTransitioning;

        protected StateMachine(ILogService logService)
        {
            this.logService = logService;
        }

        public IExitableState CurrentState => currentState;

        public void RegisterState(IExitableState state) =>
            registeredStates.Add(state.GetType(), state);

        public void Enter<TState>() where TState : class, IState =>
            EnqueueTransition(typeof(TState), state => ((IState)state).Enter());

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload> =>
            EnqueueTransition(typeof(TState), state => ((IPayloadState<TPayload>)state).Enter(payload));

        private void EnqueueTransition(Type stateType, Func<IExitableState, UniTask> enterAction)
        {
            if (!registeredStates.ContainsKey(stateType))
            {
                logService.LogError($"[{GetType().Name}] State {stateType.Name} is not registered", LogCategory.Infrastructure);
                return;
            }

            pendingTransitions.Enqueue(new Transition(stateType, enterAction));

            if (!isTransitioning)
                ProcessTransitions().Forget();
        }

        private async UniTaskVoid ProcessTransitions()
        {
            isTransitioning = true;

            try
            {
                while (pendingTransitions.TryDequeue(out var transition))
                    await ApplyTransition(transition);
            }
            finally
            {
                isTransitioning = false;
            }
        }

        private async UniTask ApplyTransition(Transition transition)
        {
            var nextState = registeredStates[transition.StateType];

            try
            {
                if (currentState != null)
                    await currentState.Exit();

                currentState = nextState;

                await transition.EnterAction(nextState);
            }
            catch (Exception e)
            {
                logService.LogError($"[{GetType().Name}] Transition to {transition.StateType.Name} failed: {e}", LogCategory.Infrastructure);
            }
        }

        private readonly struct Transition
        {
            public Type StateType { get; }
            public Func<IExitableState, UniTask> EnterAction { get; }

            public Transition(Type stateType, Func<IExitableState, UniTask> enterAction)
            {
                StateType = stateType;
                EnterAction = enterAction;
            }
        }
    }
}
