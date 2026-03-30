using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility.TabManager
{
    [Serializable]
    public struct TabData
    {
        public Tab View;
        public TabButton Button;
    }
    
    public class TabController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private List<TabData> tabsData;
        
        private TabType currentTab;

        public void LockInput() => 
            canvasGroup.interactable = false;

        public void UnlockInput() => 
            canvasGroup.interactable = true;
        
        public void Initialize()
        {
            currentTab = TabType.Home;
            
            foreach (var tabData in tabsData)
            {
                bool isCurrent = tabData.View.Type == currentTab;

                tabData.View.Initialize();
                tabData.Button.Initialize();
                tabData.Button.SetSelected(isCurrent);
                tabData.Button.OnClick += () => OnTabClicked(tabData);
                tabData.Button.OnSelected += () => OnTabSelected(tabData);

                if (isCurrent)
                    tabData.View.Show();
                else
                    tabData.View.Hide();
            } 
        }

        public void Release()
        {
            foreach (var tabData in tabsData)
            {
                bool isCurrent = tabData.View.Type == currentTab;

                tabData.Button.Release();
                tabData.Button.OnClick -= () => OnTabClicked(tabData);
                tabData.Button.OnSelected -= () => OnTabSelected(tabData);
                
                if (isCurrent)
                    tabData.View.Show();
                else
                    tabData.View.Hide();
            } 
        }

        private void OnTabClicked(TabData tabData)
        {
            //Action on Click in selected tab
        }
        
        private void OnTabSelected(TabData tabData)
        {
            var nextTab = tabsData.First(tab => tab.View.Type == tabData.View.Type);
            var selectedTab = tabsData.First(tab => tab.View.Type == currentTab);
            
            nextTab.View.Show();
            nextTab.Button.SetSelected(true);
            
            selectedTab.View.Hide();
            selectedTab.Button.SetSelected(false);

            currentTab = nextTab.View.Type;
        }
    }
}