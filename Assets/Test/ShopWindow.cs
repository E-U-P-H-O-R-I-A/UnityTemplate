using Cysharp.Threading.Tasks;
using Data.Scheme.Public;
using Services.WindowsService;
using Services.WindowsService.Windows;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Test
{
    public class ShopWindow : BaseWindow<BaseWindowParams>
    {

        [Inject] public IWindowService WindowService;
        
        
        [Space]
        [SerializeField] private Button buyButton;
        
        public override WindowType Type => WindowType.Shop;
        
        protected override UniTask OnAfterOpened(BaseWindowParams payload)
        {
            buyButton.onClick.AddListener(OnButtonClick);
            return base.OnAfterOpened(payload);
        }

        private void OnButtonClick()
        {
            WindowService.OpenSubWindow(WindowType.NoAds);
        }
    }
}