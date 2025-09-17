using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Utility.StateMachine
{
    public abstract class StateMachine 
    {
        private readonly Dictionary<Type, IState> registeredStates = new();
        
        private IState currentState;

        public async UniTask Enter<TState>() where TState : class, IState => 
            await (await ChangeState<TState>()).Enter();
        
        public void RegisterState<TState>(TState state)  where TState : IState => 
            registeredStates.Add(typeof(TState), state);

        private TState GetState<TState>() where TState : class, IState => 
            registeredStates[typeof(TState)] as TState;

        private async UniTask<TState> ChangeState<TState>() where TState : class, IState
        {
            if (currentState != null)
                await currentState.Exit();
            
            TState state = GetState<TState>();
            currentState = state;
            
            return state;
        }
    }
}