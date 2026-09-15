using Infrastructure.States;
using UnityEngine;
using UnityEngine.UI;
using Utility.StateMachine;
using VContainer;

namespace Game.UI.Lobby.HomeTab
{
    [RequireComponent(typeof(Button))]
    public class StartLevelButton : MonoBehaviour
    {
        private IStateMachine stateMachine;

        [SerializeField] private Button button;

        [Inject]
        public void Construct(IStateMachine stateMachine) =>
            this.stateMachine = stateMachine;

        public void Initialize() =>
            button.onClick.AddListener(OnClickStart);

        private void OnClickStart() =>
            stateMachine.Enter<GameplayState>();
    }
}
