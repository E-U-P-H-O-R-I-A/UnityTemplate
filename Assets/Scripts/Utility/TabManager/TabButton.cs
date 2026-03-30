using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utility.TabManager
{
    [RequireComponent(typeof(Button))]
    public class TabButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        
        [Space] 
        
        [SerializeField] private GameObject selectedState;
        [SerializeField] private GameObject unselectedState;
        
        private bool isSelected;
        
        public event Action OnSelected;
        public event Action OnClick;

        public void Initialize() => 
            Subscribe();

        public void Release() => 
            Unsubscribe();

        private void Subscribe() => 
            button.onClick.AddListener(OnButtonPressed);

        private void Unsubscribe() => 
            button.onClick.RemoveListener(OnButtonPressed);

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            selectedState.SetActive(selected);
            unselectedState.SetActive(!selected);
        }

        private void OnButtonPressed()
        {
            if (isSelected)
            {
                OnClick?.Invoke();
            }
            else
            {
                OnSelected?.Invoke();
            }
        }
    }
}