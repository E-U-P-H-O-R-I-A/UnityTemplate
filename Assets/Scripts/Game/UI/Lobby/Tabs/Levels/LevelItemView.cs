using UnityEngine;
using TMPro;
using Utility.MVC;

namespace Game.UI.Lobby.Tabs.Levels
{
    public class LevelItemView : View
    {
        private static readonly int DISABLED = Animator.StringToHash("disabled");
        private static readonly int OPEN = Animator.StringToHash("open");

        [SerializeField] private Animator animator;

        [Header("Open State")]
        [SerializeField] private GameObject openState;
        [SerializeField] private TextMeshProUGUI openIndex;

        [Header("Close State")]
        [SerializeField] private GameObject closeState;
        [SerializeField] private TextMeshProUGUI closeIndex;

        public LevelItemState CurrentState { get; private set; }

        public void Initialize(LevelItemState state, int levelNumber)
        {
            SetState(state);
            SetLevelNumber(levelNumber);
        }

        public void SetState(LevelItemState state)
        {
            CurrentState = state;

            openState.SetActive(state == LevelItemState.Open);
            closeState.SetActive(state == LevelItemState.Close);
        }

        public void SetLevelNumber(int levelNumber)
        {
            string text = levelNumber.ToString();

            openIndex.text = text;
            closeIndex.text = text;
        }

        public void PlayOpen() =>
            animator.SetTrigger(OPEN);

        public void PlayHide() =>
            animator.SetTrigger(DISABLED);
    }
}
