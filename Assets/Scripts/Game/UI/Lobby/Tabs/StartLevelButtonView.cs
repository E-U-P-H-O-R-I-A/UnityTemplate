using System;
using UnityEngine;
using UnityEngine.UI;
using Utility.MVC;

namespace Game.UI.Lobby.Tabs
{
    [RequireComponent(typeof(Button))]
    public class StartLevelButtonView : View
    {
        [SerializeField] private Button button;

        public event Action Clicked;

        public void SetInteractable(bool value) =>
            button.interactable = value;

        private void Awake() =>
            button.onClick.AddListener(OnButtonClicked);

        private void OnDestroy() =>
            button.onClick.RemoveListener(OnButtonClicked);

        private void OnButtonClicked() =>
            Clicked?.Invoke();
    }
}
