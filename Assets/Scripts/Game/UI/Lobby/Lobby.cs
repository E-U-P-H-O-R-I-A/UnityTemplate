using UnityEngine;
using Utility.TabManager;

namespace Game.UI.Lobby
{
    public class Lobby : MonoBehaviour
    {
        [SerializeField] private TabController tabController;
        
        public void Initialize()
        {
            tabController.Initialize();
        }
    }
}