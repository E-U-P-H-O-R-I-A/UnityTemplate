using System.Collections.Generic;
using Infrastructure.States;
using Utility.StateMachine;
using VContainer.Unity;

namespace Infrastructure
{
    public class GameBootstrapper : IStartable
    {
        private readonly GameStateMachine gameStateMachine;
        private readonly IEnumerable<IState> states;

        public GameBootstrapper(GameStateMachine gameStateMachine, IEnumerable<IState> states)
        {
            this.gameStateMachine = gameStateMachine;
            this.states = states;
        }

        public void Start()
        {
            foreach (var state in states)
                gameStateMachine.RegisterState(state);

            gameStateMachine.Enter<GameBootstrapState>();
        }
    }
}
