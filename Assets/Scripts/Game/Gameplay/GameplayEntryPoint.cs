using System.Collections.Generic;
using Game.Gameplay.States;
using Utility.StateMachine;
using VContainer.Unity;

namespace Game.Gameplay
{
    public class GameplayEntryPoint : IStartable
    {
        private readonly GameplayStateMachine gameplayStateMachine;
        private readonly IEnumerable<IExitableState> states;

        public GameplayEntryPoint(GameplayStateMachine gameplayStateMachine, IEnumerable<IExitableState> states)
        {
            this.gameplayStateMachine = gameplayStateMachine;
            this.states = states;
        }

        public void Start()
        {
            foreach (IExitableState state in states)
                gameplayStateMachine.RegisterState(state);

            gameplayStateMachine.Enter<LevelBuildState>();
        }
    }
}
