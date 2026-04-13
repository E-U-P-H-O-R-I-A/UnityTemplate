using System;
using Data.Scheme.Public;
using Services.WindowsService;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Test
{
    public class TEstOpenWindow : MonoBehaviour
    {
        [Inject] public IWindowService windowService;

        [SerializeField] private Button openWindow;

        private void Awake()
        {
            openWindow.onClick.AddListener(OnClokc);
        }

        private void OnClokc()
        {
            windowService.OpenWindow(WindowType.Shop);
        }
    }
}