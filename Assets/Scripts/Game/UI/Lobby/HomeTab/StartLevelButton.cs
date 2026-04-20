using Infrastructure;
using Infrastructure.States;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.UI.Lobby.HomeTab
{
    [RequireComponent(typeof(Button))]
    public class StartLevelButton : MonoBehaviour
    {
        private GameStateMachine gameStateMachine;
        
        [SerializeField] private Button button;

        [Inject]
        public void Construct(GameStateMachine gameStateMachine) => 
            this.gameStateMachine = gameStateMachine;

        public void Initialize() => 
            button.onClick.AddListener(OnClickStart);

        private void OnClickStart() => 
            gameStateMachine.Enter<GameplayState>();
    }
}