using Services.LogService;
using Sirenix.OdinInspector;
using UnityEngine;
using Utility.TabManager;
using VContainer;

namespace Game.UI.Lobby
{
    public class Lobby : MonoBehaviour
    {
        [SerializeField] private TabController tabController;
        
        private ILogService logService;

        [Inject] 
        public void Constructor(ILogService logService)
        {
            this.logService = logService;
        }
        
        public void Initialize()
        {
            tabController.Initialize();
        }

        [Button]
        public void LoadLogs()
        {
            logService.GetAllLogs();
        }

        [Button]
        public void LoadLogsCategory(LogCategory category)
        {
            logService.GetLogsByCategory(category);
        }
        
        [Button]
        public void LoadLogsCategory(LogSeverity severity)
        {
            logService.GetLogsBySeverity(severity);
        }
        
    }
}