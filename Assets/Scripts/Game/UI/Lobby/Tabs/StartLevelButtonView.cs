using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility.MVC;

namespace Game.UI.Lobby.Tabs
{
    [RequireComponent(typeof(Button))]
    public class StartLevelButtonView : View
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI label;

        public event Action Clicked;

        public void SetInteractable(bool value) =>
            button.interactable = value;

        public void SetLabel(string value) =>
            label.text = value;

        private void Awake() =>
            button.onClick.AddListener(OnButtonClicked);

        private void OnDestroy() =>
            button.onClick.RemoveListener(OnButtonClicked);

        private void OnButtonClicked() =>
            Clicked?.Invoke();
    }
}
