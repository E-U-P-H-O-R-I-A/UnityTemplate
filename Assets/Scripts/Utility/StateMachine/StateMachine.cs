using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Services.LogService;

namespace Utility.StateMachine
{
    public abstract class StateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IState> registeredStates = new();
        private readonly Queue<Type> pendingTransitions = new();
        private readonly ILogService logService;

        private IState currentState;
        private bool isTransitioning;

        protected StateMachine(ILogService logService)
        {
            this.logService = logService;
        }

        public void RegisterState<TState>(TState state) where TState : IState =>
            registeredStates.Add(typeof(TState), state);

        public void Enter<TState>() where TState : class, IState
        {
            var stateType = typeof(TState);

            if (!registeredStates.ContainsKey(stateType))
            {
                logService.LogError($"[{GetType().Name}] State {stateType.Name} is not registered", LogCategory.Infrastructure);
                return;
            }

            pendingTransitions.Enqueue(stateType);

            if (!isTransitioning)
                ProcessTransitions().Forget();
        }

        private async UniTaskVoid ProcessTransitions()
        {
            isTransitioning = true;

            try
            {
                while (pendingTransitions.TryDequeue(out var stateType))
                    await Transition(stateType);
            }
            finally
            {
                isTransitioning = false;
            }
        }

        private async UniTask Transition(Type stateType)
        {
            var nextState = registeredStates[stateType];

            try
            {
                if (currentState != null)
                    await currentState.Exit();

                currentState = nextState;

                await nextState.Enter();
            }
            catch (Exception e)
            {
                logService.LogError($"[{GetType().Name}] Transition to {stateType.Name} failed: {e}", LogCategory.Infrastructure);
            }
        }
    }
}
