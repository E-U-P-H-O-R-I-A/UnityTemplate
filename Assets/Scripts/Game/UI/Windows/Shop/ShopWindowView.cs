using Services.WindowsService.Windows;
using TMPro;
using UnityEngine;

namespace Game.UI.Windows.Shop
{
    public class ShopWindowView : WindowView<ShopWindowController>
    {
        [Header("Shop")]
        [SerializeField] private TMP_Text title;

        public void SetTitle(string value) =>
            title.text = value;
    }
}
