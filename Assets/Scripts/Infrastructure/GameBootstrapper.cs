using Infrastructure.States;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        private GameStateMachine gameStateMachine;
        private Utility.Factory.IFactory factory;

        [Inject]
        public void Construct(GameStateMachine gameStateMachine, Utility.Factory.IFactory factory)
        {
            this.gameStateMachine = gameStateMachine;
            this.factory = factory;
        }

        private void Start()
        {
            gameStateMachine.RegisterState(factory.Create<GameBootstrapState>());
            gameStateMachine.RegisterState(factory.Create<GameLoadingState>());
            gameStateMachine.RegisterState(factory.Create<GameplayState>()); 
            
            gameStateMachine.Enter<GameBootstrapState>();
            
            DontDestroyOnLoad(gameObject);
        }
        
        public class Factory : PlaceholderFactory<GameBootstrapper>
        {
        }
    }
}
