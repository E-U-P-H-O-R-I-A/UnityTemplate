using System;

namespace Utility.TabManager
{
    public class TabsModel
    {
        public event Action<TabType> Changed;

        public TabType Current { get; private set; } = TabType.Home;

        public void Select(TabType type)
        {
            if (Current == type)
                return;

            Current = type;

            Changed?.Invoke(Current);
        }
    }
}
