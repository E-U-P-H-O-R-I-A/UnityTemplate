using System;
using System.Collections.Generic;
using UnityEngine;
using Utility.MVC;

namespace Utility.TabManager
{
    [Serializable]
    public struct TabData
    {
        public TabView View;
        public TabButtonView Button;
    }

    public class TabsView : View
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private List<TabData> tabsData = new();

        public IReadOnlyList<TabData> Tabs => tabsData;

        public void SetInteractable(bool value) =>
            canvasGroup.interactable = value;
    }
}
