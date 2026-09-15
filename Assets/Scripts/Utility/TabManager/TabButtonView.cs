using System;
using UnityEngine;
using UnityEngine.UI;
using Utility.MVC;

namespace Utility.TabManager
{
    [RequireComponent(typeof(Button))]
    public class TabButtonView : View
    {
        [SerializeField] private Button button;

        [Space]

        [SerializeField] private GameObject selectedState;
        [SerializeField] private GameObject unselectedState;

        public event Action<TabButtonView> Clicked;

        public void SetSelected(bool selected)
        {
            selectedState.SetActive(selected);
            unselectedState.SetActive(!selected);
        }

        public void SetInteractable(bool value) =>
            button.interactable = value;

        private void Awake() =>
            button.onClick.AddListener(OnButtonClicked);

        private void OnDestroy() =>
            button.onClick.RemoveListener(OnButtonClicked);

        private void OnButtonClicked() =>
            Clicked?.Invoke(this);
    }
}
