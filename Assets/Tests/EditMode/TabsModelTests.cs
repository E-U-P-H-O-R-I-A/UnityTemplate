using System.Collections.Generic;
using NUnit.Framework;
using Utility.TabManager;

namespace Tests.EditMode
{
    public class TabsModelTests
    {
        [Test]
        public void DefaultTab_IsHome()
        {
            var model = new TabsModel();

            Assert.AreEqual(TabType.Home, model.Current);
        }

        [Test]
        public void Select_ChangesCurrentAndRaisesChanged()
        {
            var model = new TabsModel();
            var raised = new List<TabType>();

            model.Changed += raised.Add;
            model.Select(TabType.Market);

            Assert.AreEqual(TabType.Market, model.Current);
            CollectionAssert.AreEqual(new[] { TabType.Market }, raised);
        }

        [Test]
        public void SelectingCurrentTab_DoesNotRaiseChanged()
        {
            var model = new TabsModel();
            var raised = new List<TabType>();

            model.Select(TabType.Collection);
            model.Changed += raised.Add;
            model.Select(TabType.Collection);

            Assert.AreEqual(TabType.Collection, model.Current);
            Assert.IsEmpty(raised);
        }
    }
}
