using Infrastructure.States;
using UnityEngine;
using VContainer.Unity;

namespace Infrastructure
{
    public class GameBootstrapper : MonoBehaviour, IStartable
    {
        private GameStateMachine gameStateMachine;
        private Utility.Factory.IFactory factory;
        
        public GameBootstrapper(GameStateMachine gameStateMachine, Utility.Factory.IFactory factory)
        {
            this.gameStateMachine = gameStateMachine;
            this.factory = factory;
        }

        public void Start()
        {
            gameStateMachine.RegisterState(factory.Create<GameBootstrapState>());
            gameStateMachine.RegisterState(factory.Create<GameLoadingState>());
            gameStateMachine.RegisterState(factory.Create<GameplayState>()); 
            
            gameStateMachine.Enter<GameBootstrapState>();
        }
    }
}
