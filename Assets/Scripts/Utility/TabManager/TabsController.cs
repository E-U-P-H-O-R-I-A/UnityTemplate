using System.Collections.Generic;
using Utility.MVC;

namespace Utility.TabManager
{
    public class TabsController : Controller<TabsView>
    {
        private readonly Dictionary<TabButtonView, TabType> tabByButton = new();
        private readonly TabsModel model;

        public TabsController(TabsView view, TabsModel model) : base(view)
        {
            this.model = model;
        }

        public override void Initialize()
        {
            foreach (TabData tab in View.Tabs)
            {
                if (tab.View == null || tab.Button == null)
                    continue;

                tabByButton[tab.Button] = tab.View.Type;
                tab.Button.Clicked += OnTabButtonClicked;
            }

            model.Changed += OnCurrentTabChanged;

            Render(model.Current);
        }

        public override void Dispose()
        {
            foreach (TabButtonView button in tabByButton.Keys)
            {
                if (button != null)
                    button.Clicked -= OnTabButtonClicked;
            }

            model.Changed -= OnCurrentTabChanged;

            tabByButton.Clear();
        }

        private void OnTabButtonClicked(TabButtonView button)
        {
            if (tabByButton.TryGetValue(button, out TabType type))
                model.Select(type);
        }

        private void OnCurrentTabChanged(TabType type) =>
            Render(type);

        private void Render(TabType current)
        {
            foreach (TabData tab in View.Tabs)
            {
                if (tab.View == null || tab.Button == null)
                    continue;

                bool isCurrent = tab.View.Type == current;

                tab.Button.SetSelected(isCurrent);

                if (isCurrent)
                    tab.View.Show();
                else
                    tab.View.Hide();
            }
        }
    }
}
